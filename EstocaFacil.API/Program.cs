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

// ======================================================
// BANCO DE DADOS
// ======================================================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não configurada.");

builder.Services.AddDbContext<EstocaFacilContext>(options =>
    options.UseNpgsql(connectionString));

// ======================================================
// JWT
// ======================================================

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey não configurada.");

var issuer = jwtSettings["Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer não configurado.");

var audience = jwtSettings["Audience"]
    ?? throw new InvalidOperationException("JWT Audience não configurado.");

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

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// ======================================================
// DEPENDENCY INJECTION
// ======================================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IMovimentacaoEstoqueService, MovimentacaoEstoqueService>();
builder.Services.AddScoped<ILogService, LogService>();

builder.Services.AddScoped<IJwtTokenService>(provider =>
    new JwtTokenService(
        secretKey,
        issuer,
        audience,
        60));

// ======================================================
// HEALTH CHECKS
// ======================================================

builder.Services.AddHealthChecks();

// ======================================================
// SWAGGER
// ======================================================

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EstocaFácil API",
        Version = "v1.0.0",
        Description = "API para gerenciamento de estoque com autenticação JWT, controle de produtos e movimentações.",

        Contact = new OpenApiContact
        {
            Name = "Lisa Rios"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT no formato: Bearer {seu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
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
            Array.Empty<string>()
        }
    });
});

// ======================================================
// CONTROLLERS
// ======================================================

builder.Services.AddControllers();

// ======================================================
// CORS
// ======================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    // Em produção, substituir AllowAnyOrigin
    // pelos domínios autorizados.
});

var app = builder.Build();

// ======================================================
// MIGRATIONS E SEED
// ======================================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EstocaFacilContext>();

    context.Database.Migrate();

    await SeedData.Initialize(context);
}

// ======================================================
// PIPELINE
// ======================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "EstocaFácil API v1");

        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();
