using System;

namespace EstocaFacil.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public string Entidade { get; set; } = string.Empty;
        public int? EntidadeId { get; set; }
        public string ValoresAntigos { get; set; } = string.Empty;
        public string NovoValores { get; set; } = string.Empty;
        public DateTime DataOcorrencia { get; set; }
        public string IpAddress { get; set; } = string.Empty;

        // Navegação
        public virtual Usuario? Usuario { get; set; }
    }
}
