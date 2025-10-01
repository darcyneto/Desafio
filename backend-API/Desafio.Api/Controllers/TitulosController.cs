using Desafio.Api.Services;
using Desafio.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Desafio.Api.Controllers;

[ApiController]
[Route("api/titulos")]
public class TitulosController : ControllerBase
{
    private readonly IServicoTitulos _servico;

    public TitulosController(IServicoTitulos servico)
    {
        _servico = servico;
    }

    [HttpPost]
    public async Task<ActionResult<TituloResultado>> Post([FromBody] TituloEntrada entrada, CancellationToken cancellationToken)
    {
        Console.WriteLine($"🎯 Recebida requisição POST para criar título: {entrada.NumeroTitulo}");
        Console.WriteLine($"📋 Dados recebidos: {System.Text.Json.JsonSerializer.Serialize(entrada)}");
        
        var criado = await _servico.CriarAsync(entrada, cancellationToken);
        
        Console.WriteLine($"✅ Título criado com sucesso: {criado.NumeroTitulo}");
        return CreatedAtAction(nameof(Listar), new { numero = criado.NumeroTitulo }, criado);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TituloResultado>>> Listar(CancellationToken cancellationToken)
    {
        var lista = await _servico.ListarAsync(cancellationToken);
        return Ok(lista);
    }
}

