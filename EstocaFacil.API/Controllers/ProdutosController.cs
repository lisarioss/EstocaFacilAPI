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
    public class ProdutosController : ControllerBase
    {
        private readonly IProdutoService _produtoService;
        private readonly ILogService _logService;

        public ProdutosController(IProdutoService produtoService, ILogService logService)
        {
            _produtoService = produtoService;
            _logService = logService;
        }

        private int ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim?.Value ?? "0");
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoResponseDTO>> Criar([FromBody] ProdutoCriacaoDTO dto)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var produto = await _produtoService.CriarAsync(dto, usuarioId);
                return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoResponseDTO>> ObterPorId(int id)
        {
            try
            {
                var produto = await _produtoService.ObterPorIdAsync(id);
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ProdutoPagedResponseDTO>> Listar(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var produtos = await _produtoService.ListarAsync(pageNumber, pageSize);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("buscar/nome")]
        public async Task<ActionResult<ProdutoPagedResponseDTO>> BuscarPorNome(
            [FromQuery] string nome,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var produtos = await _produtoService.BuscarPorNomeAsync(nome, pageNumber, pageSize);
                return Ok(produtos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProdutoResponseDTO>> Atualizar(
            int id,
            [FromBody] ProdutoEdicaoDTO dto)
        {
            try
            {
                var usuarioId = ObterUsuarioId();
                var produto = await _produtoService.AtualizarAsync(id, dto, usuarioId);
                return Ok(produto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Deletar(int id)
        {
            try
            {
                await _produtoService.DeletarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
