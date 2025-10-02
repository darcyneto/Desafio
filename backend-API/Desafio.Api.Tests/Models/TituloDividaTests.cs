using Desafio.Api.Models;

namespace Desafio.Api.Tests.Models;

public class TituloDividaTests
{
    [Fact]
    public void TituloDivida_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var titulo = new TituloDivida();

        // Assert
        Assert.Equal(string.Empty, titulo.NumeroTitulo);
        Assert.Equal(string.Empty, titulo.NomeDevedor);
        Assert.Equal(string.Empty, titulo.CpfDevedor);
        Assert.Equal(0m, titulo.JurosAoMes);
        Assert.Equal(0m, titulo.MultaPercentual);
        Assert.NotNull(titulo.Parcelas);
        Assert.Empty(titulo.Parcelas);
    }

    [Fact]
    public void TituloDivida_ValorOriginal_ComParcelasVazias_DeveRetornarZero()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            Parcelas = new List<ParcelaDivida>()
        };

        // Act
        var valorOriginal = titulo.ValorOriginal;

        // Assert
        Assert.Equal(0m, valorOriginal);
    }

    [Fact]
    public void TituloDivida_ValorOriginal_ComUmaParcela_DeveRetornarValorDaParcela()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            Parcelas = new List<ParcelaDivida>
            {
                new() { Valor = 1000m }
            }
        };

        // Act
        var valorOriginal = titulo.ValorOriginal;

        // Assert
        Assert.Equal(1000m, valorOriginal);
    }

    [Fact]
    public void TituloDivida_ValorOriginal_ComMultiplasParcelas_DeveRetornarSomaDosValores()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            Parcelas = new List<ParcelaDivida>
            {
                new() { Valor = 500m },
                new() { Valor = 750m },
                new() { Valor = 1250m }
            }
        };

        // Act
        var valorOriginal = titulo.ValorOriginal;

        // Assert
        Assert.Equal(2500m, valorOriginal);
    }

    [Fact]
    public void TituloDivida_ValorOriginal_ComParcelasComValorZero_DeveRetornarSomaCorreta()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            Parcelas = new List<ParcelaDivida>
            {
                new() { Valor = 0m },
                new() { Valor = 100m },
                new() { Valor = 0m }
            }
        };

        // Act
        var valorOriginal = titulo.ValorOriginal;

        // Assert
        Assert.Equal(100m, valorOriginal);
    }

    [Fact]
    public void TituloDivida_ValorOriginal_ComParcelasComValoresDecimais_DeveRetornarSomaPrecisa()
    {
        // Arrange
        var titulo = new TituloDivida
        {
            Parcelas = new List<ParcelaDivida>
            {
                new() { Valor = 99.99m },
                new() { Valor = 0.01m }
            }
        };

        // Act
        var valorOriginal = titulo.ValorOriginal;

        // Assert
        Assert.Equal(100.00m, valorOriginal);
    }

    [Fact]
    public void TituloDivida_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var titulo = new TituloDivida();

        // Act
        titulo.NumeroTitulo = "12345";
        titulo.NomeDevedor = "João Silva";
        titulo.CpfDevedor = "12345678901";
        titulo.JurosAoMes = 0.01m;
        titulo.MultaPercentual = 0.02m;

        // Assert
        Assert.Equal("12345", titulo.NumeroTitulo);
        Assert.Equal("João Silva", titulo.NomeDevedor);
        Assert.Equal("12345678901", titulo.CpfDevedor);
        Assert.Equal(0.01m, titulo.JurosAoMes);
        Assert.Equal(0.02m, titulo.MultaPercentual);
    }
}

public class ParcelaDividaTests
{
    [Fact]
    public void ParcelaDivida_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var parcela = new ParcelaDivida();

        // Assert
        Assert.Equal(0, parcela.Numero);
        Assert.Equal(string.Empty, parcela.Vencimento);
        Assert.Equal(0m, parcela.Valor);
    }

    [Fact]
    public void ParcelaDivida_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Numero = 1;
        parcela.Vencimento = "2025-12-31";
        parcela.Valor = 1000m;

        // Assert
        Assert.Equal(1, parcela.Numero);
        Assert.Equal("2025-12-31", parcela.Vencimento);
        Assert.Equal(1000m, parcela.Valor);
    }

    [Fact]
    public void ParcelaDivida_ComValoresDecimais_DeveManterPrecisao()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Valor = 123.456789m;

        // Assert
        Assert.Equal(123.456789m, parcela.Valor);
    }

    [Fact]
    public void ParcelaDivida_ComValorNegativo_DevePermitir()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Valor = -100m;

        // Assert
        Assert.Equal(-100m, parcela.Valor);
    }

    [Fact]
    public void ParcelaDivida_ComNumeroNegativo_DevePermitir()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Numero = -1;

        // Assert
        Assert.Equal(-1, parcela.Numero);
    }

    [Theory]
    [InlineData("2025-12-31")]
    [InlineData("2025-01-01")]
    [InlineData("2024-02-29")] // Ano bissexto
    [InlineData("2023-12-25")]
    public void ParcelaDivida_ComVencimentosValidos_DeveAceitar(string vencimento)
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Vencimento = vencimento;

        // Assert
        Assert.Equal(vencimento, parcela.Vencimento);
    }

    [Fact]
    public void ParcelaDivida_ComVencimentoVazio_DevePermitir()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Vencimento = "";

        // Assert
        Assert.Equal("", parcela.Vencimento);
    }

    [Fact]
    public void ParcelaDivida_ComVencimentoNull_DevePermitir()
    {
        // Arrange
        var parcela = new ParcelaDivida();

        // Act
        parcela.Vencimento = null!;

        // Assert
        Assert.Null(parcela.Vencimento);
    }
}
