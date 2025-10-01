using System.Linq;
using Desafio.Api.Models;
using Desafio.Api.Repositories;
using Desafio.Api.Services;
using Desafio.Api.ViewModels;

namespace Desafio.Api.Tests;

public class ServicoTitulosTests
{
    private readonly ITituloRepositorio _repositorio;
    private readonly ServicoTitulos _servico;

    public ServicoTitulosTests()
    {
        _repositorio = new TituloRepositorioMemoria();
        _servico = new ServicoTitulos(_repositorio);
    }

    [Fact]
    public async Task CriarAsync_DeveGerarValorAtualizado()
    {
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "101010",
            NomeDevedor = "Fulano",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas =
            {
                new ParcelaEntrada { Numero = 10, Vencimento = hoje.AddDays(-73).ToString("yyyy-MM-dd"), Valor = 100m },
                new ParcelaEntrada { Numero = 11, Vencimento = hoje.AddDays(-42).ToString("yyyy-MM-dd"), Valor = 100m },
                new ParcelaEntrada { Numero = 12, Vencimento = hoje.AddDays(-11).ToString("yyyy-MM-dd"), Valor = 100m }
            }
        };

        var resultado = await _servico.CriarAsync(entrada, CancellationToken.None);

        Assert.Equal(300m, resultado.ValorOriginal);
        Assert.True(resultado.ValorAtualizado > resultado.ValorOriginal);
        Assert.Equal(3, resultado.ParcelaQuantidade);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarItensCadastrados()
    {
        var titulo = new TituloDivida
        {
            NumeroTitulo = "2020",
            NomeDevedor = "Teste",
            CpfDevedor = "10987654321",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas =
            {
                new ParcelaDivida { Numero = 1, Vencimento = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)).ToString("yyyy-MM-dd"), Valor = 100m }
            }
        };

        await _repositorio.AdicionarAsync(titulo, CancellationToken.None);

        var lista = await _servico.ListarAsync(CancellationToken.None);

        Assert.Single(lista);
        Assert.Equal("2020", lista.First().NumeroTitulo);
    }
}
