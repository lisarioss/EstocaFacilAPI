using System;
using System.Threading.Tasks;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EstocaFacilContext _context;
        private IUsuarioRepository _usuarioRepository;
        private IProdutoRepository _produtoRepository;
        private IMovimentacaoEstoqueRepository _movimentacaoRepository;
        private ILogRepository _logRepository;

        public UnitOfWork(EstocaFacilContext context)
        {
            _context = context;
        }

        public IUsuarioRepository Usuarios => _usuarioRepository ??= new UsuarioRepository(_context);
        public IProdutoRepository Produtos => _produtoRepository ??= new ProdutoRepository(_context);
        public IMovimentacaoEstoqueRepository Movimentacoes => _movimentacaoRepository ??= new MovimentacaoEstoqueRepository(_context);
        public ILogRepository Logs => _logRepository ??= new LogRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
