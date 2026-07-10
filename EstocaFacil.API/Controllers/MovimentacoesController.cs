using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EstocaFacil.Application.DTOs;
using EstocaFacil.Application.Services;
using System.Security.Claims;

namespace EstocaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMovimentacaoEstoqueService _movimentacaoService;
        private readonly ILogService _logService;

        public MovimentacoesController(IMovimentacaoEstoqueService movimentacaoService, ILogService logService)
        {
            _movimentacaoService = movimentacaoService;
            _logService = logService;
        }

        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        private string ObterIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        }

        [HttpPost]
        public async Task<ActionResult<MovimentacaoResponseDTO>> RegistrarMovimentacao(
            [FromBody] MovimentacaoEstoqueDTO dto)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var ipAddress = ObterIpAddress();
                var movimentacao = await _movimentacaoService.RegistrarMovimentacaoAsync(dto, usuarioId, ipAddress);
                return CreatedAtAction(nameof(ObterMovimentacoesProduto), new { produtoId = dto.ProdutoId }, movimentacao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("produto/{produtoId}")]
        public async Task<ActionResult<IEnumerable<MovimentacaoResponseDTO>>> ObterMovimentacoesProduto(int produtoId)
        {
            try
            {
                var movimentacoes = await _movimentacaoService.ObterMovimentacoesProdutoAsync(produtoId);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("data-range")]
        public async Task<ActionResult<IEnumerable<MovimentacaoResponseDTO>>> ObterMovimentacoesPorData(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim)
        {
            try
            {
                var movimentacoes = await _movimentacaoService.ObterMovimentacoesPorDataAsync(dataInicio, dataFim);
                return Ok(movimentacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
