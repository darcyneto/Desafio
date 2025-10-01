using Desafio.Api.Models;
using Desafio.Api.Repositories;
using Desafio.Api.ViewModels;

namespace Desafio.Api.Services;

public class ServicoTitulos : IServicoTitulos
{
    private readonly ITituloRepositorio _repositorio;

    public ServicoTitulos(ITituloRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<TituloResultado> CriarAsync(TituloEntrada entrada, CancellationToken cancellationToken)
    {
        var titulo = ConverterParaModelo(entrada);
        await _repositorio.AdicionarAsync(titulo, cancellationToken);
        return CalcularResultado(titulo, DateOnly.FromDateTime(DateTime.Today));
    }

    public async Task<IReadOnlyCollection<TituloResultado>> ListarAsync(CancellationToken cancellationToken)
    {
        var itens = await _repositorio.ObterTodosAsync(cancellationToken);
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        return itens.Select(t => CalcularResultado(t, hoje)).ToList();
    }

    private static TituloDivida ConverterParaModelo(TituloEntrada entrada)
    {
        return new TituloDivida
        {
            NumeroTitulo = entrada.NumeroTitulo,
            NomeDevedor = entrada.NomeDevedor,
            CpfDevedor = entrada.CpfDevedor,
            JurosAoMes = entrada.JurosAoMes,
            MultaPercentual = entrada.MultaPercentual,
            Parcelas = entrada.Parcelas.Select(p => new ParcelaDivida
            {
                Numero = p.Numero,
                Vencimento = p.Vencimento,
                Valor = p.Valor
            }).ToList()
        };
    }

    private static TituloResultado CalcularResultado(TituloDivida titulo, DateOnly referencia)
    {
        var resultado = new TituloResultado
        {
            NumeroTitulo = titulo.NumeroTitulo,
            NomeDevedor = titulo.NomeDevedor,
            CpfDevedor = titulo.CpfDevedor,
            JurosAoMes = titulo.JurosAoMes,
            MultaPercentual = titulo.MultaPercentual,
            ParcelaQuantidade = titulo.Parcelas.Count,
            ValorOriginal = titulo.ValorOriginal
        };

        decimal totalAtualizado = 0;

        foreach (var parcela in titulo.Parcelas.OrderBy(p => p.Numero))
        {
            var vencimento = DateOnly.TryParse(parcela.Vencimento, out var dataParcela) ? dataParcela : referencia;
            var diasAtraso = (referencia.ToDateTime(TimeOnly.MinValue) - vencimento.ToDateTime(TimeOnly.MinValue)).Days;
            diasAtraso = Math.Max(diasAtraso, 0);

            var jurosDia = titulo.JurosAoMes / 30m;
            var valorJuros = diasAtraso * jurosDia * parcela.Valor;
            var valorMulta = parcela.Valor * titulo.MultaPercentual;
            var valorAtualizado = parcela.Valor + valorJuros + valorMulta;

            totalAtualizado += valorAtualizado;

            resultado.Parcelas.Add(new ParcelaResultado
            {
                Numero = parcela.Numero,
                Vencimento = vencimento.ToString("yyyy-MM-dd"),
                Valor = parcela.Valor,
                DiasEmAtraso = diasAtraso,
                ValorAtualizado = decimal.Round(valorAtualizado, 2)
            });
        }

        resultado.ValorAtualizado = decimal.Round(totalAtualizado, 2);
        return resultado;
    }
}

