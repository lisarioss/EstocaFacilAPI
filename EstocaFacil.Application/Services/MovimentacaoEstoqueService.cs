using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EstocaFacil.Domain.Entities;
using EstocaFacil.Domain.Repositories;
using EstocaFacil.Application.DTOs;

namespace EstocaFacil.Application.Services
{
    public interface IMovimentacaoEstoqueService
    {
        Task<MovimentacaoResponseDTO> RegistrarMovimentacaoAsync(MovimentacaoEstoqueDTO dto, int usuarioId, string ipAddress);
        Task<IEnumerable<MovimentacaoResponseDTO>> ObterMovimentacoesProdutoAsync(int produtoId);
        Task<IEnumerable<MovimentacaoResponseDTO>> ObterMovimentacoesPorDataAsync(DateTime dataInicio, DateTime dataFim);
    }

    public class MovimentacaoEstoqueService : IMovimentacaoEstoqueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService _logService;

        public MovimentacaoEstoqueService(IUnitOfWork unitOfWork, ILogService logService)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
        }

        public async Task<MovimentacaoResponseDTO> RegistrarMovimentacaoAsync(MovimentacaoEstoqueDTO dto, int usuarioId, string ipAddress)
        {
            var produto = await _unitOfWork.Produtos.GetByIdAsync(dto.ProdutoId);
            if (produto == null)
                throw new Exception("Produto não encontrado");

            // Validar quantidade
            if (dto.Tipo == TipoMovimentacao.Saida && produto.QuantidadeEstoque < dto.Quantidade)
                throw new Exception("Quantidade insuficiente em estoque");

            // Atualizar quantidade
            if (dto.Tipo == TipoMovimentacao.Entrada)
                produto.QuantidadeEstoque += dto.Quantidade;
            else if (dto.Tipo == TipoMovimentacao.Saida)
                produto.QuantidadeEstoque -= dto.Quantidade;
            else if (dto.Tipo == TipoMovimentacao.Devolucao)
                produto.QuantidadeEstoque += dto.Quantidade;
            else if (dto.Tipo == TipoMovimentacao.Ajuste)
                produto.QuantidadeEstoque = dto.Quantidade;

            await _unitOfWork.Produtos.UpdateAsync(produto);

            // Registrar movimentação
            var movimentacao = new MovimentacaoEstoque
            {
                ProdutoId = dto.ProdutoId,
                Tipo = dto.Tipo,
                Quantidade = dto.Quantidade,
                Observacao = dto.Observacao,
                Data = DateTime.UtcNow,
                UsuarioId = usuarioId,
                DataCriacao = DateTime.UtcNow
            };

            await _unitOfWork.Movimentacoes.AddAsync(movimentacao);
            await _unitOfWork.SaveChangesAsync();

            // Registrar log
            var descricaoLog = $"Movimentação {dto.Tipo}: {dto.Quantidade} unidades";
            await _logService.RegistrarLogAsync(usuarioId, descricaoLog, "Movimentacao", movimentacao.Id, null, null, ipAddress);

            return new MovimentacaoResponseDTO
            {
                Id = movimentacao.Id,
                ProdutoId = movimentacao.ProdutoId,
                ProdutoNome = produto.Nome,
                Tipo = movimentacao.Tipo,
                Quantidade = movimentacao.Quantidade,
                Observacao = movimentacao.Observacao,
                Data = movimentacao.Data,
                UsuarioNome = ""
            };
        }

        public async Task<IEnumerable<MovimentacaoResponseDTO>> ObterMovimentacoesProdutoAsync(int produtoId)
        {
            var movimentacoes = await _unitOfWork.Movimentacoes.GetByProdutoIdAsync(produtoId);
            var produto = await _unitOfWork.Produtos.GetByIdAsync(produtoId);

            return movimentacoes.Select(m => new MovimentacaoResponseDTO
            {
                Id = m.Id,
                ProdutoId = m.ProdutoId,
                ProdutoNome = produto?.Nome ?? "",
                Tipo = m.Tipo,
                Quantidade = m.Quantidade,
                Observacao = m.Observacao,
                Data = m.Data
            });
        }

        public async Task<IEnumerable<MovimentacaoResponseDTO>> ObterMovimentacoesPorDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            var movimentacoes = await _unitOfWork.Movimentacoes.GetByDataRangeAsync(dataInicio, dataFim);

            return movimentacoes.Select(m => new MovimentacaoResponseDTO
            {
                Id = m.Id,
                ProdutoId = m.ProdutoId,
                ProdutoNome = m.Produto?.Nome ?? "",
                Tipo = m.Tipo,
                Quantidade = m.Quantidade,
                Observacao = m.Observacao,
                Data = m.Data
            });
        }
    }
}
