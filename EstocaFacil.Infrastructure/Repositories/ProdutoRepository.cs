using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Infrastructure.Repositories
{
    public class ProdutoRepository : RepositoryBase<Produto>, IProdutoRepository
    {
        public ProdutoRepository(EstocaFacilContext context) : base(context)
        {
        }

        public async Task<Produto> GetByCodigoAsync(string codigo)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<IEnumerable<Produto>> SearchByNomeAsync(string nome, int pageNumber, int pageSize)
        {
            var query = _dbSet.Where(p => p.Nome.Contains(nome) && p.Ativo);
            
            var produtos = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return produtos;
        }
    }
}
