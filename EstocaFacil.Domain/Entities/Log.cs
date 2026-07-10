using System;

namespace EstocaFacil.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Acao { get; set; }
        public string Entidade { get; set; }
        public int? EntidadeId { get; set; }
        public string ValoresAntigos { get; set; }
        public string NovoValores { get; set; }
        public DateTime DataOcorrencia { get; set; }
        public string IpAddress { get; set; }

        // Navegação
        public virtual Usuario Usuario { get; set; }
    }
}
