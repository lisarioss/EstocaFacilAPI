using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstocaFacil.Application.Services;
using EstocaFacil.Domain.Entities;
using System.Security.Claims;

namespace EstocaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogsController(ILogService logService)
        {
            _logService = logService;
        }

        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        [HttpGet("usuario")]
        public async Task<ActionResult<IEnumerable<Log>>> ObterLogsPorUsuario()
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var logs = await _logService.ObterLogsPorUsuarioAsync(usuarioId);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("entidade/{entidade}")]
        public async Task<ActionResult<IEnumerable<Log>>> ObterLogsPorEntidade(string entidade)
        {
            try
            {
                var logs = await _logService.ObterLogsPorEntidadeAsync(entidade);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
