using Desafio.Api.Models;

namespace Desafio.Api.Repositories;

public class TituloRepositorioMemoria : ITituloRepositorio
{
    private readonly List<TituloDivida> _titulos = new();

    public Task AdicionarAsync(TituloDivida titulo, CancellationToken cancellationToken)
    {
        _titulos.Add(titulo);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<TituloDivida>> ObterTodosAsync(CancellationToken cancellationToken)
    {
        var copia = _titulos.Select(t => new TituloDivida
        {
            NumeroTitulo = t.NumeroTitulo,
            NomeDevedor = t.NomeDevedor,
            CpfDevedor = t.CpfDevedor,
            JurosAoMes = t.JurosAoMes,
            MultaPercentual = t.MultaPercentual,
            Parcelas = t.Parcelas.Select(p => new ParcelaDivida
            {
                Numero = p.Numero,
                Vencimento = p.Vencimento,
                Valor = p.Valor
            }).ToList()
        }).ToList();

        return Task.FromResult((IReadOnlyCollection<TituloDivida>)copia);
    }
}

