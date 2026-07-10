using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;
using System;
using System.Threading.Tasks;
using BCrypt.Net;

namespace EstocaFacil.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task Initialize(EstocaFacilContext context)
        {
            // Aplicar qualquer migration pendente
            await context.Database.MigrateAsync();

            // Se já existe dados, não fazer seed novamente
            if (await context.Usuarios.AnyAsync())
            {
                return;
            }

            // Criar usuários de teste
            var usuarios = new Usuario[]
            {
                new Usuario
                {
                    Nome = "Admin Sistema",
                    Email = "admin@estocafacil.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    DataCriacao = DateTime.UtcNow,
                    Ativo = true
                },
                new Usuario
                {
                    Nome = "Gerente Estoque",
                    Email = "gerente@estocafacil.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("gerente123"),
                    DataCriacao = DateTime.UtcNow,
                    Ativo = true
                },
                new Usuario
                {
                    Nome = "Operador Logo",
                    Email = "operador@estocafacil.com",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("operador123"),
                    DataCriacao = DateTime.UtcNow,
                    Ativo = true
                }
            };

            await context.Usuarios.AddRangeAsync(usuarios);
            await context.SaveChangesAsync();

            // Criar produtos de exemplo
            var produtos = new Produto[]
            {
                new Produto
                {
                    Codigo = "PROD001",
                    Nome = "Notebook Dell Inspiron",
                    Descricao = "Notebook com processador Intel Core i5, 8GB RAM, 256GB SSD",
                    Preco = 2999.99m,
                    QuantidadeEstoque = 15,
                    QuantidadeMinima = 5,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioCriacaoId = usuarios[0].Id,
                    Ativo = true
                },
                new Produto
                {
                    Codigo = "PROD002",
                    Nome = "Mouse Logitech MX Master",
                    Descricao = "Mouse sem fio com tecnologia avançada",
                    Preco = 249.90m,
                    QuantidadeEstoque = 50,
                    QuantidadeMinima = 10,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioCriacaoId = usuarios[0].Id,
                    Ativo = true
                },
                new Produto
                {
                    Codigo = "PROD003",
                    Nome = "Teclado Mecânico RGB",
                    Descricao = "Teclado mecânico com iluminação RGB programável",
                    Preco = 349.90m,
                    QuantidadeEstoque = 25,
                    QuantidadeMinima = 5,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioCriacaoId = usuarios[0].Id,
                    Ativo = true
                },
                new Produto
                {
                    Codigo = "PROD004",
                    Nome = "Monitor LG 27\"",
                    Descricao = "Monitor IPS 27 polegadas Full HD 75Hz",
                    Preco = 899.90m,
                    QuantidadeEstoque = 10,
                    QuantidadeMinima = 3,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioCriacaoId = usuarios[0].Id,
                    Ativo = true
                },
                new Produto
                {
                    Codigo = "PROD005",
                    Nome = "Webcam HD 1080p",
                    Descricao = "Webcam com microfone embutido e foco automático",
                    Preco = 149.90m,
                    QuantidadeEstoque = 30,
                    QuantidadeMinima = 5,
                    DataCriacao = DateTime.UtcNow,
                    UsuarioCriacaoId = usuarios[0].Id,
                    Ativo = true
                }
            };

            await context.Produtos.AddRangeAsync(produtos);
            await context.SaveChangesAsync();

            // Criar alguns logs de teste
            var logs = new Log[]
            {
                new Log
                {
                    Acao = "Sistema inicializado com sucesso",
                    Entidade = "Sistema",
                    DataOcorrencia = DateTime.UtcNow,
                    UsuarioId = usuarios[0].Id
                },
                new Log
                {
                    Acao = "Produtos de teste inseridos no banco",
                    Entidade = "Produto",
                    DataOcorrencia = DateTime.UtcNow,
                    UsuarioId = usuarios[0].Id
                }
            };

            await context.Logs.AddRangeAsync(logs);
            await context.SaveChangesAsync();
        }
    }
}
