using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Infrastructure.Repositories
{
    public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(EstocaFacilContext context) : base(context)
        {
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
