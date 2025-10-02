using Desafio.Api.ViewModels;

namespace Desafio.Api.Tests.ViewModels;

public class TituloEntradaTests
{
    [Fact]
    public void TituloEntrada_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var titulo = new TituloEntrada();

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
    public void TituloEntrada_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var titulo = new TituloEntrada();
        var parcelas = new List<ParcelaEntrada>
        {
            new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000m }
        };

        // Act
        titulo.NumeroTitulo = "12345";
        titulo.NomeDevedor = "João Silva";
        titulo.CpfDevedor = "12345678901";
        titulo.JurosAoMes = 0.01m;
        titulo.MultaPercentual = 0.02m;
        titulo.Parcelas = parcelas;

        // Assert
        Assert.Equal("12345", titulo.NumeroTitulo);
        Assert.Equal("João Silva", titulo.NomeDevedor);
        Assert.Equal("12345678901", titulo.CpfDevedor);
        Assert.Equal(0.01m, titulo.JurosAoMes);
        Assert.Equal(0.02m, titulo.MultaPercentual);
        Assert.Single(titulo.Parcelas);
        Assert.Equal(1, titulo.Parcelas.First().Numero);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("TÍTULO-12345")]
    [InlineData("ABC-2025-001")]
    public void TituloEntrada_ComDiferentesFormatosDeNumero_DeveAceitar(string numeroTitulo)
    {
        // Arrange
        var titulo = new TituloEntrada();

        // Act
        titulo.NumeroTitulo = numeroTitulo;

        // Assert
        Assert.Equal(numeroTitulo, titulo.NumeroTitulo);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(0.01)]
    [InlineData(0.1)]
    [InlineData(1.0)]
    [InlineData(-0.01)] // Taxa negativa (pode ser válida em alguns contextos)
    public void TituloEntrada_ComDiferentesTaxas_DeveAceitar(double taxa)
    {
        // Arrange
        var titulo = new TituloEntrada();

        // Act
        titulo.JurosAoMes = (decimal)taxa;
        titulo.MultaPercentual = (decimal)taxa;

        // Assert
        Assert.Equal((decimal)taxa, titulo.JurosAoMes);
        Assert.Equal((decimal)taxa, titulo.MultaPercentual);
    }
}

public class ParcelaEntradaTests
{
    [Fact]
    public void ParcelaEntrada_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var parcela = new ParcelaEntrada();

        // Assert
        Assert.Equal(0, parcela.Numero);
        Assert.Equal(string.Empty, parcela.Vencimento);
        Assert.Equal(0m, parcela.Valor);
    }

    [Fact]
    public void ParcelaEntrada_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var parcela = new ParcelaEntrada();

        // Act
        parcela.Numero = 1;
        parcela.Vencimento = "2025-12-31";
        parcela.Valor = 1000m;

        // Assert
        Assert.Equal(1, parcela.Numero);
        Assert.Equal("2025-12-31", parcela.Vencimento);
        Assert.Equal(1000m, parcela.Valor);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ParcelaEntrada_ComDiferentesNumeros_DeveAceitar(int numero)
    {
        // Arrange
        var parcela = new ParcelaEntrada();

        // Act
        parcela.Numero = numero;

        // Assert
        Assert.Equal(numero, parcela.Numero);
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(100.0)]
    [InlineData(999.99)]
    [InlineData(-100.0)] // Valor negativo (pode ser válido em alguns contextos)
    public void ParcelaEntrada_ComDiferentesValores_DeveAceitar(double valor)
    {
        // Arrange
        var parcela = new ParcelaEntrada();

        // Act
        parcela.Valor = (decimal)valor;

        // Assert
        Assert.Equal((decimal)valor, parcela.Valor);
    }

    [Theory]
    [InlineData("2025-12-31")]
    [InlineData("2025-01-01")]
    [InlineData("2024-02-29")] // Ano bissexto
    [InlineData("")]
    public void ParcelaEntrada_ComDiferentesVencimentos_DeveAceitar(string vencimento)
    {
        // Arrange
        var parcela = new ParcelaEntrada();

        // Act
        parcela.Vencimento = vencimento;

        // Assert
        Assert.Equal(vencimento, parcela.Vencimento);
    }
}

public class TituloResultadoTests
{
    [Fact]
    public void TituloResultado_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var titulo = new TituloResultado();

        // Assert
        Assert.Equal(string.Empty, titulo.NumeroTitulo);
        Assert.Equal(string.Empty, titulo.NomeDevedor);
        Assert.Equal(string.Empty, titulo.CpfDevedor);
        Assert.Equal(0m, titulo.JurosAoMes);
        Assert.Equal(0m, titulo.MultaPercentual);
        Assert.Equal(0m, titulo.ValorOriginal);
        Assert.Equal(0m, titulo.ValorAtualizado);
        Assert.Equal(0, titulo.ParcelaQuantidade);
        Assert.NotNull(titulo.Parcelas);
        Assert.Empty(titulo.Parcelas);
    }

    [Fact]
    public void TituloResultado_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var titulo = new TituloResultado();
        var parcelas = new List<ParcelaResultado>
        {
            new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000m, DiasEmAtraso = 5, ValorAtualizado = 1050m }
        };

        // Act
        titulo.NumeroTitulo = "12345";
        titulo.NomeDevedor = "João Silva";
        titulo.CpfDevedor = "12345678901";
        titulo.JurosAoMes = 0.01m;
        titulo.MultaPercentual = 0.02m;
        titulo.ValorOriginal = 1000m;
        titulo.ValorAtualizado = 1050m;
        titulo.ParcelaQuantidade = 1;
        titulo.Parcelas = parcelas;

        // Assert
        Assert.Equal("12345", titulo.NumeroTitulo);
        Assert.Equal("João Silva", titulo.NomeDevedor);
        Assert.Equal("12345678901", titulo.CpfDevedor);
        Assert.Equal(0.01m, titulo.JurosAoMes);
        Assert.Equal(0.02m, titulo.MultaPercentual);
        Assert.Equal(1000m, titulo.ValorOriginal);
        Assert.Equal(1050m, titulo.ValorAtualizado);
        Assert.Equal(1, titulo.ParcelaQuantidade);
        Assert.Single(titulo.Parcelas);
    }

    [Fact]
    public void TituloResultado_ValorAtualizadoMaiorQueOriginal_DeveSerValido()
    {
        // Arrange
        var titulo = new TituloResultado();

        // Act
        titulo.ValorOriginal = 1000m;
        titulo.ValorAtualizado = 1050m;

        // Assert
        Assert.True(titulo.ValorAtualizado > titulo.ValorOriginal);
    }

    [Fact]
    public void TituloResultado_ValorAtualizadoIgualAoOriginal_DeveSerValido()
    {
        // Arrange
        var titulo = new TituloResultado();

        // Act
        titulo.ValorOriginal = 1000m;
        titulo.ValorAtualizado = 1000m;

        // Assert
        Assert.Equal(titulo.ValorOriginal, titulo.ValorAtualizado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    public void TituloResultado_ComDiferentesQuantidadesDeParcelas_DeveAceitar(int quantidade)
    {
        // Arrange
        var titulo = new TituloResultado();

        // Act
        titulo.ParcelaQuantidade = quantidade;

        // Assert
        Assert.Equal(quantidade, titulo.ParcelaQuantidade);
    }
}

public class ParcelaResultadoTests
{
    [Fact]
    public void ParcelaResultado_Constructor_DeveInicializarPropriedades()
    {
        // Act
        var parcela = new ParcelaResultado();

        // Assert
        Assert.Equal(0, parcela.Numero);
        Assert.Equal(string.Empty, parcela.Vencimento);
        Assert.Equal(0m, parcela.Valor);
        Assert.Equal(0, parcela.DiasEmAtraso);
        Assert.Equal(0m, parcela.ValorAtualizado);
    }

    [Fact]
    public void ParcelaResultado_Propriedades_DevePermitirAtribuicao()
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.Numero = 1;
        parcela.Vencimento = "2025-12-31";
        parcela.Valor = 1000m;
        parcela.DiasEmAtraso = 5;
        parcela.ValorAtualizado = 1050m;

        // Assert
        Assert.Equal(1, parcela.Numero);
        Assert.Equal("2025-12-31", parcela.Vencimento);
        Assert.Equal(1000m, parcela.Valor);
        Assert.Equal(5, parcela.DiasEmAtraso);
        Assert.Equal(1050m, parcela.ValorAtualizado);
    }

    [Fact]
    public void ParcelaResultado_ComDiasEmAtraso_DeveSerValido()
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.DiasEmAtraso = 30;
        parcela.Valor = 1000m;
        parcela.ValorAtualizado = 1200m;

        // Assert
        Assert.Equal(30, parcela.DiasEmAtraso);
        Assert.True(parcela.ValorAtualizado > parcela.Valor);
    }

    [Fact]
    public void ParcelaResultado_ComZeroDiasEmAtraso_DeveSerValido()
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.DiasEmAtraso = 0;
        parcela.Valor = 1000m;
        parcela.ValorAtualizado = 1000m;

        // Assert
        Assert.Equal(0, parcela.DiasEmAtraso);
        Assert.Equal(parcela.Valor, parcela.ValorAtualizado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(365)]
    [InlineData(-1)] // Dias negativos (pode ser válido em alguns contextos)
    public void ParcelaResultado_ComDiferentesDiasEmAtraso_DeveAceitar(int diasEmAtraso)
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.DiasEmAtraso = diasEmAtraso;

        // Assert
        Assert.Equal(diasEmAtraso, parcela.DiasEmAtraso);
    }

    [Fact]
    public void ParcelaResultado_ValorAtualizadoMaiorQueOriginal_DeveSerValido()
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.Valor = 1000m;
        parcela.ValorAtualizado = 1150m;

        // Assert
        Assert.True(parcela.ValorAtualizado > parcela.Valor);
    }

    [Fact]
    public void ParcelaResultado_ValorAtualizadoIgualAoOriginal_DeveSerValido()
    {
        // Arrange
        var parcela = new ParcelaResultado();

        // Act
        parcela.Valor = 1000m;
        parcela.ValorAtualizado = 1000m;

        // Assert
        Assert.Equal(parcela.Valor, parcela.ValorAtualizado);
    }
}
