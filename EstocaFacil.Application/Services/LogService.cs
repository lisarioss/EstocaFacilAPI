using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;

namespace EstocaFacil.Application.Services
{
    public interface ILogService
    {
        Task RegistrarLogAsync(int usuarioId, string acao, string entidade, int? entidadeId, string valoresAntigos, string novosValores, string ipAddress = null);
        Task<IEnumerable<Log>> ObterLogsPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Log>> ObterLogsPorEntidadeAsync(string entidade);
    }

    public class LogService : ILogService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task RegistrarLogAsync(int usuarioId, string acao, string entidade, int? entidadeId, string valoresAntigos, string novosValores, string ipAddress = null)
        {
            var log = new Log
            {
                UsuarioId = usuarioId,
                Acao = acao,
                Entidade = entidade,
                EntidadeId = entidadeId,
                ValoresAntigos = valoresAntigos,
                NovoValores = novosValores,
                DataOcorrencia = DateTime.UtcNow,
                IpAddress = ipAddress ?? "0.0.0.0"
            };

            await _unitOfWork.Logs.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Log>> ObterLogsPorUsuarioAsync(int usuarioId)
        {
            return await _unitOfWork.Logs.GetByUsuarioIdAsync(usuarioId);
        }

        public async Task<IEnumerable<Log>> ObterLogsPorEntidadeAsync(string entidade)
        {
            return await _unitOfWork.Logs.GetByEntidadeAsync(entidade);
        }
    }
}
