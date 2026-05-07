using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Infrastructure.Repositories
{
    public class LogRepository : RepositoryBase<Log>, ILogRepository
    {
        public LogRepository(EstocaFacilContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Log>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet
                .Where(l => l.UsuarioId == usuarioId)
                .OrderByDescending(l => l.DataOcorrencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByEntidadeAsync(string entidade)
        {
            return await _dbSet
                .Where(l => l.Entidade == entidade)
                .OrderByDescending(l => l.DataOcorrencia)
                .ToListAsync();
        }
    }
}
