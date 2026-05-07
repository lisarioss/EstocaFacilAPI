using System;
using EstocaFacil.Domain.Entities;

namespace EstocaFacil.Application.DTOs
{
    public class MovimentacaoEstoqueDTO
    {
        public int ProdutoId { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
    }

    public class MovimentacaoResponseDTO
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string ProdutoNome { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public DateTime Data { get; set; }
        public string UsuarioNome { get; set; }
    }
}
