using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EstocaFacil.Domain.Entities;

namespace EstocaFacil.Domain.Repositories
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }

    public interface IUsuarioRepository : IRepositoryBase<Usuario>
    {
        Task<Usuario> GetByEmailAsync(string email);
    }

    public interface IProdutoRepository : IRepositoryBase<Produto>
    {
        Task<Produto> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Produto>> SearchByNomeAsync(string nome, int pageNumber, int pageSize);
    }

    public interface IMovimentacaoEstoqueRepository : IRepositoryBase<MovimentacaoEstoque>
    {
        Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId);
        Task<IEnumerable<MovimentacaoEstoque>> GetByDataRangeAsync(DateTime dataInicio, DateTime dataFim);
    }

    public interface ILogRepository : IRepositoryBase<Log>
    {
        Task<IEnumerable<Log>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Log>> GetByEntidadeAsync(string entidade);
    }

    public interface IUnitOfWork : IDisposable
    {
        IUsuarioRepository Usuarios { get; }
        IProdutoRepository Produtos { get; }
        IMovimentacaoEstoqueRepository Movimentacoes { get; }
        ILogRepository Logs { get; }
        Task<int> SaveChangesAsync();
    }
}
