using Desafio.Api.Models;
using Desafio.Api.Repositories;

namespace Desafio.Api.Tests.Repositories;

public class TituloRepositorioMemoriaTests
{
    private readonly TituloRepositorioMemoria _repositorio;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public TituloRepositorioMemoriaTests()
    {
        _repositorio = new TituloRepositorioMemoria();
    }

    [Fact]
    public async Task AdicionarAsync_ComTituloValido_DeveAdicionarTitulo()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida>
            {
                new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000m }
            }
        };

        // Act
        await _repositorio.AdicionarAsync(titulo, _cancellationToken);

        // Assert
        var titulos = await _repositorio.ObterTodosAsync(_cancellationToken);
        Assert.Single(titulos);
        Assert.Equal("12345", titulos.First().NumeroTitulo);
    }

    [Fact]
    public async Task AdicionarAsync_ComMultiplosTitulos_DeveAdicionarTodos()
    {
        // Arrange
        var titulo1 = new TituloDivida
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida>()
        };

        var titulo2 = new TituloDivida
        {
            NumeroTitulo = "67890",
            NomeDevedor = "Maria Santos",
            CpfDevedor = "98765432100",
            JurosAoMes = 0.015m,
            MultaPercentual = 0.025m,
            Parcelas = new List<ParcelaDivida>()
        };

        // Act
        await _repositorio.AdicionarAsync(titulo1, _cancellationToken);
        await _repositorio.AdicionarAsync(titulo2, _cancellationToken);

        // Assert
        var titulos = await _repositorio.ObterTodosAsync(_cancellationToken);
        Assert.Equal(2, titulos.Count);
        Assert.Contains(titulos, t => t.NumeroTitulo == "12345");
        Assert.Contains(titulos, t => t.NumeroTitulo == "67890");
    }

    [Fact]
    public async Task AdicionarAsync_ComTituloNull_DeveLancarArgumentNullException()
    {
        // Arrange
        TituloDivida? titulo = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _repositorio.AdicionarAsync(titulo!, _cancellationToken));
    }

    [Fact]
    public async Task ObterTodosAsync_ComRepositorioVazio_DeveRetornarListaVazia()
    {
        // Act
        var titulos = await _repositorio.ObterTodosAsync(_cancellationToken);

        // Assert
        Assert.Empty(titulos);
    }

    [Fact]
    public async Task ObterTodosAsync_ComTitulosAdicionados_DeveRetornarCopiaDosTitulos()
    {
        // Arrange
        var tituloOriginal = new TituloDivida
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida>
            {
                new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000m },
                new() { Numero = 2, Vencimento = "2026-01-31", Valor = 1000m }
            }
        };

        await _repositorio.AdicionarAsync(tituloOriginal, _cancellationToken);

        // Act
        var titulosRetornados = await _repositorio.ObterTodosAsync(_cancellationToken);
        var tituloRetornado = titulosRetornados.First();

        // Assert
        Assert.Single(titulosRetornados);
        Assert.Equal(tituloOriginal.NumeroTitulo, tituloRetornado.NumeroTitulo);
        Assert.Equal(tituloOriginal.NomeDevedor, tituloRetornado.NomeDevedor);
        Assert.Equal(tituloOriginal.CpfDevedor, tituloRetornado.CpfDevedor);
        Assert.Equal(tituloOriginal.JurosAoMes, tituloRetornado.JurosAoMes);
        Assert.Equal(tituloOriginal.MultaPercentual, tituloRetornado.MultaPercentual);
        Assert.Equal(2, tituloRetornado.Parcelas.Count);
        
        // Verificar se é uma cópia (não a mesma referência)
        Assert.NotSame(tituloOriginal, tituloRetornado);
        Assert.NotSame(tituloOriginal.Parcelas, tituloRetornado.Parcelas);
    }

    [Fact]
    public async Task ObterTodosAsync_DeveRetornarCopiaProfundaDasParcelas()
    {
        // Arrange
        var parcelaOriginal = new ParcelaDivida
        {
            Numero = 1,
            Vencimento = "2025-12-31",
            Valor = 1000m
        };

        var tituloOriginal = new TituloDivida
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida> { parcelaOriginal }
        };

        await _repositorio.AdicionarAsync(tituloOriginal, _cancellationToken);

        // Act
        var titulosRetornados = await _repositorio.ObterTodosAsync(_cancellationToken);
        var parcelaRetornada = titulosRetornados.First().Parcelas.First();

        // Assert
        Assert.Equal(parcelaOriginal.Numero, parcelaRetornada.Numero);
        Assert.Equal(parcelaOriginal.Vencimento, parcelaRetornada.Vencimento);
        Assert.Equal(parcelaOriginal.Valor, parcelaRetornada.Valor);
        
        // Verificar se é uma cópia (não a mesma referência)
        Assert.NotSame(parcelaOriginal, parcelaRetornada);
    }

    [Fact]
    public async Task AdicionarAsync_ComCancellationTokenCancelado_DeveCompletarSemErro()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida>()
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        // O método é síncrono internamente, então não deve lançar exceção
        await _repositorio.AdicionarAsync(titulo, cancellationTokenSource.Token);
    }

    [Fact]
    public async Task ObterTodosAsync_ComCancellationTokenCancelado_DeveCompletarSemErro()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        // O método é síncrono internamente, então não deve lançar exceção
        var titulos = await _repositorio.ObterTodosAsync(cancellationTokenSource.Token);
        Assert.NotNull(titulos);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("TÍTULO-12345")]
    [InlineData("ABC-2025-001")]
    public async Task AdicionarAsync_ComDiferentesFormatosDeNumero_DeveAceitar(string numeroTitulo)
    {
        // Arrange
        var titulo = new TituloDivida
        {
            NumeroTitulo = numeroTitulo,
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaDivida>()
        };

        // Act
        await _repositorio.AdicionarAsync(titulo, _cancellationToken);

        // Assert
        var titulos = await _repositorio.ObterTodosAsync(_cancellationToken);
        Assert.Contains(titulos, t => t.NumeroTitulo == numeroTitulo);
    }

    [Fact]
    public async Task AdicionarAsync_ComTituloComTodasPropriedadesPreenchidas_DevePreservarDados()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            NumeroTitulo = "TIT-2025-001",
            NomeDevedor = "João da Silva Santos",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.015m,
            MultaPercentual = 0.025m,
            Parcelas = new List<ParcelaDivida>
            {
                new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000.50m },
                new() { Numero = 2, Vencimento = "2026-01-31", Valor = 1000.50m },
                new() { Numero = 3, Vencimento = "2026-02-28", Valor = 1000.50m }
            }
        };

        // Act
        await _repositorio.AdicionarAsync(titulo, _cancellationToken);

        // Assert
        var titulos = await _repositorio.ObterTodosAsync(_cancellationToken);
        var tituloRetornado = titulos.First();
        
        Assert.Equal("TIT-2025-001", tituloRetornado.NumeroTitulo);
        Assert.Equal("João da Silva Santos", tituloRetornado.NomeDevedor);
        Assert.Equal("12345678901", tituloRetornado.CpfDevedor);
        Assert.Equal(0.015m, tituloRetornado.JurosAoMes);
        Assert.Equal(0.025m, tituloRetornado.MultaPercentual);
        Assert.Equal(3, tituloRetornado.Parcelas.Count);
        Assert.Equal(3001.50m, tituloRetornado.ValorOriginal);
    }
}
