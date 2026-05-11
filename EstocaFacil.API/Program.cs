using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using EstocaFacil.Infrastructure.Data;
using EstocaFacil.Infrastructure.Repositories;
using EstocaFacil.Domain.Repositories;
using EstocaFacil.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// ==================== CONFIGURAÇÃO DE BANCO DE DADOS ====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=estocafacil;Username=postgres;Password=postgres";

builder.Services.AddDbContext<EstocaFacilContext>(options =>
    options.UseNpgsql(connectionString));

// ==================== CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ====================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "sua-chave-super-secreta-com-pelo-menos-32-caracteres-aqui!";
var issuer = jwtSettings["Issuer"] ?? "EstocaFacilAPI";
var audience = jwtSettings["Audience"] ?? "EstocaFacilUI";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// ==================== CONFIGURAÇÃO DE SERVIÇOS ====================
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<IJwtTokenService>(provider => 
    new JwtTokenService(secretKey, issuer, audience, 60));

// ==================== CONFIGURAÇÃO DE SWAGGER ====================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EstocaFácil API",
        Version = "v1.0.0",
        Description = "API para Gerenciamento de Estoque com autenticação segura, controle de produtos e movimentação.",
        Contact = new OpenApiContact
        {
            Name = "EstocaFácil",
            Email = "dev@estocafacil.com"
        }
    });

    // Configurar autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// ==================== SEED DE DADOS ====================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EstocaFacilContext>();
    await SeedData.Initialize(context);
}

// ==================== CONFIGURAÇÃO DO PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EstocaFácil API v1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
