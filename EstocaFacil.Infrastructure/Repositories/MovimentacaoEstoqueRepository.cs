using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Infrastructure.Repositories
{
    public class MovimentacaoEstoqueRepository : RepositoryBase<MovimentacaoEstoque>, IMovimentacaoEstoqueRepository
    {
        public MovimentacaoEstoqueRepository(EstocaFacilContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> GetByProdutoIdAsync(int produtoId)
        {
            return await _dbSet
                .Where(m => m.ProdutoId == produtoId)
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<MovimentacaoEstoque>> GetByDataRangeAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Where(m => m.Data >= dataInicio && m.Data <= dataFim)
                .OrderByDescending(m => m.Data)
                .ToListAsync();
        }
    }
}
