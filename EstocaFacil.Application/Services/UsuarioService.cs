using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
            if (usuario == null || !VerificaSenha(senha, usuario.SenhaHash))
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
            using (var sha256 = SHA256.Create())
            {
                var senhaBytes = Encoding.UTF8.GetBytes(senha);
                var hashBytes = sha256.ComputeHash(senhaBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        private bool VerificaSenha(string senha, string hash)
        {
            var hashDaSenhaFornecida = HashSenha(senha);
            return hashDaSenhaFornecida == hash;
        }
    }
}
