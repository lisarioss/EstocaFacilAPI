using System;

namespace EstocaFacil.Domain.Entities
{
    public enum TipoMovimentacao
    {
        Entrada = 1,
        Saida = 2,
        Devolucao = 3,
        Ajuste = 4
    }

    public class MovimentacaoEstoque
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DataCriacao { get; set; }

        // Navegações
        public virtual Produto Produto { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
