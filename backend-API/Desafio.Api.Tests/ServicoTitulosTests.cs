using System.Linq;
using Desafio.Api.Models;
using Desafio.Api.Repositories;
using Desafio.Api.Services;
using Desafio.Api.ViewModels;
using Moq;

namespace Desafio.Api.Tests;

public class ServicoTitulosTests
{
    private readonly Mock<ITituloRepositorio> _mockRepositorio;
    private readonly ServicoTitulos _servico;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ServicoTitulosTests()
    {
        _mockRepositorio = new Mock<ITituloRepositorio>();
        _servico = new ServicoTitulos(_mockRepositorio.Object);
    }

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarResultadoCalculado()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "101010",
            NomeDevedor = "Fulano",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 10, Vencimento = hoje.AddDays(-73).ToString("yyyy-MM-dd"), Valor = 100m },
                new() { Numero = 11, Vencimento = hoje.AddDays(-42).ToString("yyyy-MM-dd"), Valor = 100m },
                new() { Numero = 12, Vencimento = hoje.AddDays(-11).ToString("yyyy-MM-dd"), Valor = 100m }
            }
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.CriarAsync(entrada, _cancellationToken);

        // Assert
        Assert.Equal(300m, resultado.ValorOriginal);
        Assert.True(resultado.ValorAtualizado > resultado.ValorOriginal);
        Assert.Equal(3, resultado.ParcelaQuantidade);
        Assert.Equal("101010", resultado.NumeroTitulo);
        Assert.Equal("Fulano", resultado.NomeDevedor);
        Assert.Equal("12345678901", resultado.CpfDevedor);
        Assert.Equal(0.01m, resultado.JurosAoMes);
        Assert.Equal(0.02m, resultado.MultaPercentual);
        
        _mockRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComRepositorioLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "101010",
            NomeDevedor = "Fulano",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>()
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro no repositório"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _servico.CriarAsync(entrada, _cancellationToken));
    }

    [Fact]
    public async Task CriarAsync_ComEntradaNull_DeveLancarArgumentNullException()
    {
        // Arrange
        TituloEntrada? entrada = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _servico.CriarAsync(entrada!, _cancellationToken));
    }

    [Fact]
    public async Task ListarAsync_ComRepositorioVazio_DeveRetornarListaVazia()
    {
        // Arrange
        _mockRepositorio
            .Setup(r => r.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(new List<TituloDivida>());

        // Act
        var lista = await _servico.ListarAsync(_cancellationToken);

        // Assert
        Assert.Empty(lista);
        _mockRepositorio.Verify(r => r.ObterTodosAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_ComItensNoRepositorio_DeveRetornarListaCalculada()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var titulos = new List<TituloDivida>
        {
            new()
            {
                NumeroTitulo = "2020",
                NomeDevedor = "Teste",
                CpfDevedor = "10987654321",
                JurosAoMes = 0.01m,
                MultaPercentual = 0.02m,
                Parcelas = new List<ParcelaDivida>
                {
                    new() { Numero = 1, Vencimento = hoje.AddDays(-5).ToString("yyyy-MM-dd"), Valor = 100m }
                }
            },
            new()
            {
                NumeroTitulo = "2021",
                NomeDevedor = "Outro Teste",
                CpfDevedor = "11111111111",
                JurosAoMes = 0.015m,
                MultaPercentual = 0.025m,
                Parcelas = new List<ParcelaDivida>
                {
                    new() { Numero = 1, Vencimento = hoje.AddDays(-10).ToString("yyyy-MM-dd"), Valor = 200m }
                }
            }
        };

        _mockRepositorio
            .Setup(r => r.ObterTodosAsync(_cancellationToken))
            .ReturnsAsync(titulos);

        // Act
        var lista = await _servico.ListarAsync(_cancellationToken);

        // Assert
        Assert.Equal(2, lista.Count);
        Assert.Equal("2020", lista.First().NumeroTitulo);
        Assert.Equal("2021", lista.Last().NumeroTitulo);
        Assert.True(lista.First().ValorAtualizado > 100m);
        Assert.True(lista.Last().ValorAtualizado > 200m);
    }

    [Fact]
    public async Task ListarAsync_ComRepositorioLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        _mockRepositorio
            .Setup(r => r.ObterTodosAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao listar"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _servico.ListarAsync(_cancellationToken));
    }

    [Fact]
    public void Constructor_ComRepositorioNull_DeveLancarArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ServicoTitulos(null!));
    }

    [Fact]
    public void Constructor_ComRepositorioValido_DeveCriarInstancia()
    {
        // Arrange
        var mockRepositorio = new Mock<ITituloRepositorio>();

        // Act
        var servico = new ServicoTitulos(mockRepositorio.Object);

        // Assert
        Assert.NotNull(servico);
    }

    [Theory]
    [InlineData(0.01, 0.02, 100, 5)] // 5 dias de atraso
    [InlineData(0.005, 0.01, 1000, 30)] // 30 dias de atraso
    [InlineData(0.02, 0.05, 500, 15)] // 15 dias de atraso
    public async Task CriarAsync_ComDiferentesTaxas_DeveCalcularCorretamente(
        double jurosAoMes, double multaPercentual, double valorParcela, int diasAtraso)
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "TESTE",
            NomeDevedor = "Teste",
            CpfDevedor = "12345678901",
            JurosAoMes = (decimal)jurosAoMes,
            MultaPercentual = (decimal)multaPercentual,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 1, Vencimento = hoje.AddDays(-diasAtraso).ToString("yyyy-MM-dd"), Valor = (decimal)valorParcela }
            }
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.CriarAsync(entrada, _cancellationToken);

        // Assert
        var parcela = resultado.Parcelas.First();
        Assert.Equal(diasAtraso, parcela.DiasEmAtraso);
        Assert.True(parcela.ValorAtualizado > parcela.Valor);
        Assert.True(resultado.ValorAtualizado > resultado.ValorOriginal);
    }

    [Fact]
    public async Task CriarAsync_ComParcelaVencidaNoFuturo_DeveTerZeroDiasAtraso()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "FUTURO",
            NomeDevedor = "Teste",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 1, Vencimento = hoje.AddDays(10).ToString("yyyy-MM-dd"), Valor = 100m }
            }
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.CriarAsync(entrada, _cancellationToken);

        // Assert
        var parcela = resultado.Parcelas.First();
        Assert.Equal(0, parcela.DiasEmAtraso);
        // Valor atualizado deve ser igual ao original + multa (sem juros)
        Assert.True(parcela.ValorAtualizado >= parcela.Valor);
    }

    [Fact]
    public async Task CriarAsync_ComDataInvalida_DeveUsarDataReferencia()
    {
        // Arrange
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "DATA_INVALIDA",
            NomeDevedor = "Teste",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 1, Vencimento = "data-invalida", Valor = 100m }
            }
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.CriarAsync(entrada, _cancellationToken);

        // Assert
        var parcela = resultado.Parcelas.First();
        Assert.Equal(0, parcela.DiasEmAtraso); // Data inválida é tratada como hoje
        Assert.True(parcela.ValorAtualizado >= parcela.Valor);
    }

    [Fact]
    public async Task CriarAsync_ComMultiplasParcelas_DeveCalcularCadaParcelaSeparadamente()
    {
        // Arrange
        var hoje = DateOnly.FromDateTime(DateTime.Today);
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "MULTIPLAS",
            NomeDevedor = "Teste",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 1, Vencimento = hoje.AddDays(-30).ToString("yyyy-MM-dd"), Valor = 100m },
                new() { Numero = 2, Vencimento = hoje.AddDays(-15).ToString("yyyy-MM-dd"), Valor = 200m },
                new() { Numero = 3, Vencimento = hoje.AddDays(10).ToString("yyyy-MM-dd"), Valor = 300m }
            }
        };

        _mockRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<TituloDivida>(), _cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _servico.CriarAsync(entrada, _cancellationToken);

        // Assert
        Assert.Equal(3, resultado.Parcelas.Count);
        Assert.Equal(600m, resultado.ValorOriginal);
        
        var parcela1 = resultado.Parcelas.First(p => p.Numero == 1);
        var parcela2 = resultado.Parcelas.First(p => p.Numero == 2);
        var parcela3 = resultado.Parcelas.First(p => p.Numero == 3);
        
        Assert.Equal(30, parcela1.DiasEmAtraso);
        Assert.Equal(15, parcela2.DiasEmAtraso);
        Assert.Equal(0, parcela3.DiasEmAtraso);
        
        Assert.True(parcela1.ValorAtualizado > parcela1.Valor);
        Assert.True(parcela2.ValorAtualizado > parcela2.Valor);
        Assert.True(parcela3.ValorAtualizado >= parcela3.Valor);
    }
}
