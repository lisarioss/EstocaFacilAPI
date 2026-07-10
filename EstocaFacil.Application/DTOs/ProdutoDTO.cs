namespace EstocaFacil.Application.DTOs
{
    public class ProdutoCriacaoDTO
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeMinima { get; set; }
    }

    public class ProdutoEdicaoDTO
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeMinima { get; set; }
        public bool Ativo { get; set; }
    }

    public class ProdutoResponseDTO
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int QuantidadeMinima { get; set; }
        public bool Ativo { get; set; }
    }

    public class ProdutoPagedResponseDTO
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
        public System.Collections.Generic.List<ProdutoResponseDTO> Items { get; set; }
    }
}
