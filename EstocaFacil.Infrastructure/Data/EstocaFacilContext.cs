using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;

namespace EstocaFacil.Infrastructure.Data
{
    public class EstocaFacilContext : DbContext
    {
        public EstocaFacilContext(DbContextOptions<EstocaFacilContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<MovimentacaoEstoque> Movimentacoes { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração de Usuario
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id);
            
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configuração de Produto
            modelBuilder.Entity<Produto>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Codigo)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Preco)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Produto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            // Configuração de MovimentacaoEstoque
            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasOne(m => m.Produto)
                .WithMany()
                .HasForeignKey(m => m.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasOne(m => m.Usuario)
                .WithMany()
                .HasForeignKey(m => m.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de Log
            modelBuilder.Entity<Log>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.Usuario)
                .WithMany()
                .HasForeignKey(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Log>()
                .Property(l => l.Entidade)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
