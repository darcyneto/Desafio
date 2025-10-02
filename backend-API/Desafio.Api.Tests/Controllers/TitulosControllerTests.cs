using Desafio.Api.Controllers;
using Desafio.Api.Services;
using Desafio.Api.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace Desafio.Api.Tests.Controllers;

public class TitulosControllerTests
{
    private readonly Mock<IServicoTitulos> _mockServico;
    private readonly TitulosController _controller;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public TitulosControllerTests()
    {
        _mockServico = new Mock<IServicoTitulos>();
        _controller = new TitulosController(_mockServico.Object);
        
        // Setup controller context
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Post_ComDadosValidos_DeveRetornarCreatedResult()
    {
        // Arrange
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>
            {
                new() { Numero = 1, Vencimento = "2025-12-31", Valor = 1000m }
            }
        };

        var resultadoEsperado = new TituloResultado
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            ValorOriginal = 1000m,
            ValorAtualizado = 1050m,
            ParcelaQuantidade = 1
        };

        _mockServico
            .Setup(s => s.CriarAsync(entrada, _cancellationToken))
            .ReturnsAsync(resultadoEsperado);

        // Act
        var resultado = await _controller.Post(entrada, _cancellationToken);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado.Result);
        Assert.Equal(nameof(TitulosController.Listar), createdResult.ActionName);
        Assert.Equal("12345", createdResult.RouteValues?["numero"]);
        Assert.Equal(resultadoEsperado, createdResult.Value);
        
        _mockServico.Verify(s => s.CriarAsync(entrada, _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Post_ComServicoLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        var entrada = new TituloEntrada
        {
            NumeroTitulo = "12345",
            NomeDevedor = "João Silva",
            CpfDevedor = "12345678901",
            JurosAoMes = 0.01m,
            MultaPercentual = 0.02m,
            Parcelas = new List<ParcelaEntrada>()
        };

        _mockServico
            .Setup(s => s.CriarAsync(entrada, _cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro interno"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Post(entrada, _cancellationToken));
    }

    [Fact]
    public async Task Post_ComEntradaNull_DeveAceitarValorNull()
    {
        // Arrange
        TituloEntrada? entrada = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _controller.Post(entrada!, _cancellationToken));
    }

    [Fact]
    public async Task Listar_ComDadosExistentes_DeveRetornarOkResult()
    {
        // Arrange
        var titulosEsperados = new List<TituloResultado>
        {
            new()
            {
                NumeroTitulo = "12345",
                NomeDevedor = "João Silva",
                CpfDevedor = "12345678901",
                JurosAoMes = 0.01m,
                MultaPercentual = 0.02m,
                ValorOriginal = 1000m,
                ValorAtualizado = 1050m,
                ParcelaQuantidade = 1
            },
            new()
            {
                NumeroTitulo = "67890",
                NomeDevedor = "Maria Santos",
                CpfDevedor = "98765432100",
                JurosAoMes = 0.015m,
                MultaPercentual = 0.025m,
                ValorOriginal = 2000m,
                ValorAtualizado = 2100m,
                ParcelaQuantidade = 2
            }
        };

        _mockServico
            .Setup(s => s.ListarAsync(_cancellationToken))
            .ReturnsAsync(titulosEsperados);

        // Act
        var resultado = await _controller.Listar(_cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var titulosRetornados = Assert.IsAssignableFrom<IReadOnlyCollection<TituloResultado>>(okResult.Value);
        Assert.Equal(2, titulosRetornados.Count);
        Assert.Equal("12345", titulosRetornados.First().NumeroTitulo);
        Assert.Equal("67890", titulosRetornados.Last().NumeroTitulo);
        
        _mockServico.Verify(s => s.ListarAsync(_cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Listar_ComListaVazia_DeveRetornarOkResultVazio()
    {
        // Arrange
        var titulosVazios = new List<TituloResultado>();

        _mockServico
            .Setup(s => s.ListarAsync(_cancellationToken))
            .ReturnsAsync(titulosVazios);

        // Act
        var resultado = await _controller.Listar(_cancellationToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado.Result);
        var titulosRetornados = Assert.IsAssignableFrom<IReadOnlyCollection<TituloResultado>>(okResult.Value);
        Assert.Empty(titulosRetornados);
    }

    [Fact]
    public async Task Listar_ComServicoLancandoExcecao_DevePropagarExcecao()
    {
        // Arrange
        _mockServico
            .Setup(s => s.ListarAsync(_cancellationToken))
            .ThrowsAsync(new InvalidOperationException("Erro ao listar"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Listar(_cancellationToken));
    }

    [Fact]
    public void Constructor_ComServicoNull_DeveLancarArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TitulosController(null!));
    }

    [Fact]
    public void Constructor_ComServicoValido_DeveCriarInstancia()
    {
        // Arrange
        var mockServico = new Mock<IServicoTitulos>();

        // Act
        var controller = new TitulosController(mockServico.Object);

        // Assert
        Assert.NotNull(controller);
    }
}
