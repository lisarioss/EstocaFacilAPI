using Microsoft.AspNetCore.Mvc;
using EstocaFacil.Application.DTOs;
using EstocaFacil.Application.Services;

namespace EstocaFacil.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogService _logService;

        public AuthController(IUsuarioService usuarioService, IJwtTokenService jwtTokenService, ILogService logService)
        {
            _usuarioService = usuarioService;
            _jwtTokenService = jwtTokenService;
            _logService = logService;
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<UsuarioResponseDTO>> Registrar([FromBody] UsuarioCriacaoDTO dto)
        {
            try
            {
                var usuario = await _usuarioService.RegistrarAsync(dto);
                return Ok(new { message = "Usuário registrado com sucesso", data = usuario });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] UsuarioLoginDTO dto)
        {
            try
            {
                var usuario = await _usuarioService.AutenticarAsync(dto.Email, dto.Senha);
                
                var token = _jwtTokenService.GenerarToken(usuario);
                var refreshToken = _jwtTokenService.GenerarRefreshToken();

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
                await _logService.RegistrarLogAsync(usuario.Id, "LOGIN", "Usuario", usuario.Id, null, null, ipAddress);

                var response = new TokenResponseDTO
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Usuario = new UsuarioResponseDTO
                    {
                        Id = usuario.Id,
                        Nome = usuario.Nome,
                        Email = usuario.Email,
                        Ativo = usuario.Ativo
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpGet("perfil")]
        public async Task<ActionResult<UsuarioResponseDTO>> ObterPerfil()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Token inválido" });

                if (int.TryParse(userIdClaim.Value, out int usuarioId))
                {
                    var usuario = await _usuarioService.ObterPorIdAsync(usuarioId);
                    return Ok(usuario);
                }

                return Unauthorized(new { message = "Token inválido" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
