namespace EstocaFacil.Application.DTOs
{
    public class UsuarioLoginDTO
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class UsuarioCriacaoDTO
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
    }

    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public bool Ativo { get; set; }
    }

    public class TokenResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UsuarioResponseDTO Usuario { get; set; }
    }
}
