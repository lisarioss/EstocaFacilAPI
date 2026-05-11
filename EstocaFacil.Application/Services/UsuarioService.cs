using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;
using EstocaFacil.Application.DTOs;

namespace EstocaFacil.Application.Services
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO> RegistrarAsync(UsuarioCriacaoDTO dto);
        Task<Usuario> ObterPorEmailAsync(string email);
        Task<Usuario> AutenticarAsync(string email, string senha);
        Task<UsuarioResponseDTO> ObterPorIdAsync(int id);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UsuarioService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UsuarioResponseDTO> RegistrarAsync(UsuarioCriacaoDTO dto)
        {
            // Verificar se email já existe
            var usuarioExistente = await _unitOfWork.Usuarios.GetByEmailAsync(dto.Email);
            if (usuarioExistente != null)
                throw new Exception("Email já cadastrado");

            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = HashSenha(dto.Senha),
                Ativo = true,
                DataCriacao = DateTime.UtcNow
            };

            await _unitOfWork.Usuarios.AddAsync(usuario);
            await _unitOfWork.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ativo = usuario.Ativo
            };
        }

        public async Task<Usuario> AutenticarAsync(string email, string senha)
        {
            var usuario = await _unitOfWork.Usuarios.GetByEmailAsync(email);
            if (usuario == null)
                throw new Exception("Email ou senha inválido");

            if (!await VerificaSenhaAsync(senha, usuario))
                throw new Exception("Email ou senha inválido");

            return usuario;
        }

        public async Task<Usuario> ObterPorEmailAsync(string email)
        {
            return await _unitOfWork.Usuarios.GetByEmailAsync(email);
        }

        public async Task<UsuarioResponseDTO> ObterPorIdAsync(int id)
        {
            var usuario = await _unitOfWork.Usuarios.GetByIdAsync(id);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Ativo = usuario.Ativo
            };
        }

        private string HashSenha(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        private async Task<bool> VerificaSenhaAsync(string senha, Usuario usuario)
        {
            var hash = usuario.SenhaHash;
            if (string.IsNullOrWhiteSpace(hash))
                return false;

            if (hash.StartsWith("$2a$") || hash.StartsWith("$2b$") || hash.StartsWith("$2y$"))
            {
                return BCrypt.Net.BCrypt.Verify(senha, hash);
            }

            if (hash.StartsWith("sha256:"))
            {
                var antigaHash = hash.Substring("sha256:".Length);
                if (VerificaSenhaSha256(senha, antigaHash))
                {
                    usuario.SenhaHash = HashSenha(senha);
                    await _unitOfWork.Usuarios.UpdateAsync(usuario);
                    await _unitOfWork.SaveChangesAsync();
                    return true;
                }
            }

            return false;
        }

        private bool VerificaSenhaSha256(string senha, string hash)
        {
            using (var sha256 = SHA256.Create())
            {
                var senhaBytes = Encoding.UTF8.GetBytes(senha);
                var hashBytes = sha256.ComputeHash(senhaBytes);
                var hashCalculo = Convert.ToBase64String(hashBytes);
                return hashCalculo == hash;
            }
        }
    }
}
