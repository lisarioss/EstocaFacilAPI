using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;
using EstocaFacil.Application.DTOs;

namespace EstocaFacil.Application.Services
{
    public interface IProdutoService
    {
        Task<ProdutoResponseDTO> CriarAsync(ProdutoCriacaoDTO dto, int usuarioId);
        Task<ProdutoResponseDTO> AtualizarAsync(int id, ProdutoEdicaoDTO dto, int usuarioId);
        Task<ProdutoResponseDTO> ObterPorIdAsync(int id);
        Task<ProdutoPagedResponseDTO> ListarAsync(int pageNumber = 1, int pageSize = 10);
        Task<ProdutoPagedResponseDTO> BuscarPorNomeAsync(string nome, int pageNumber = 1, int pageSize = 10);
        Task DeletarAsync(int id);
    }

    public class ProdutoService : IProdutoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;

        public ProdutoService(IUnitOfWork unitOfWork, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
        }

        public async Task<ProdutoResponseDTO> CriarAsync(ProdutoCriacaoDTO dto, int usuarioId)
        {
            // Verificar se código já existe
            var produtoExistente = await _unitOfWork.Produtos.GetByCodigoAsync(dto.Codigo);
            if (produtoExistente != null)
                throw new Exception("Código de produto já existe");

            var produto = new Produto
            {
                Codigo = dto.Codigo,
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                Preco = dto.Preco,
                QuantidadeEstoque = 0,
                QuantidadeMinima = dto.QuantidadeMinima,
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                UsuarioCriacaoId = usuarioId
            };

            await _unitOfWork.Produtos.AddAsync(produto);
            await _unitOfWork.SaveChangesAsync();

            await _logService.RegistrarLogAsync(usuarioId, "CREATE", "Produto", produto.Id, null, SerializarProduto(produto));

            return MapearParaResponseDTO(produto);
        }

        public async Task<ProdutoResponseDTO> AtualizarAsync(int id, ProdutoEdicaoDTO dto, int usuarioId)
        {
            var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
            if (produto == null)
                throw new Exception("Produto não encontrado");

            var valoresAntigos = SerializarProduto(produto);

            produto.Nome = dto.Nome;
            produto.Descricao = dto.Descricao;
            produto.Preco = dto.Preco;
            produto.QuantidadeMinima = dto.QuantidadeMinima;
            produto.Ativo = dto.Ativo;
            produto.DataAtualizacao = DateTime.UtcNow;

            await _unitOfWork.Produtos.UpdateAsync(produto);
            await _unitOfWork.SaveChangesAsync();

            await _logService.RegistrarLogAsync(usuarioId, "UPDATE", "Produto", produto.Id, valoresAntigos, SerializarProduto(produto));

            return MapearParaResponseDTO(produto);
        }

        public async Task<ProdutoResponseDTO> ObterPorIdAsync(int id)
        {
            var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
            if (produto == null)
                throw new Exception("Produto não encontrado");

            return MapearParaResponseDTO(produto);
        }

        public async Task<ProdutoPagedResponseDTO> ListarAsync(int pageNumber = 1, int pageSize = 10)
        {
            var produtos = await _unitOfWork.Produtos.GetAllAsync();
            var produtosAtivos = produtos.Where(p => p.Ativo).ToList();

            var totalItems = produtosAtivos.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = produtosAtivos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapearParaResponseDTO)
                .ToList();

            return new ProdutoPagedResponseDTO
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };
        }

        public async Task<ProdutoPagedResponseDTO> BuscarPorNomeAsync(string nome, int pageNumber = 1, int pageSize = 10)
        {
            var produtos = await _unitOfWork.Produtos.SearchByNomeAsync(nome, pageNumber, pageSize);
            var produtosList = produtos.ToList();

            var totalItems = produtosList.Count;
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var items = produtosList
                .Select(MapearParaResponseDTO)
                .ToList();

            return new ProdutoPagedResponseDTO
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };
        }

        public async Task DeletarAsync(int id)
        {
            var produto = await _unitOfWork.Produtos.GetByIdAsync(id);
            if (produto == null)
                throw new Exception("Produto não encontrado");

            await _logService.RegistrarLogAsync(0, "DELETE", "Produto", id, SerializarProduto(produto), null);
            await _unitOfWork.Produtos.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        private ProdutoResponseDTO MapearParaResponseDTO(Produto produto)
        {
            return new ProdutoResponseDTO
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                QuantidadeMinima = produto.QuantidadeMinima,
                Ativo = produto.Ativo
            };
        }

        private string SerializarProduto(Produto produto)
        {
            return $"Código: {produto.Codigo}, Nome: {produto.Nome}, Preço: {produto.Preco}, QtdEstoque: {produto.QuantidadeEstoque}";
        }
    }
}
