using Desafio.Api.Models;

namespace Desafio.Api.Repositories;

public interface ITituloRepositorio
{
    Task AdicionarAsync(TituloDivida titulo, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TituloDivida>> ObterTodosAsync(CancellationToken cancellationToken);
}

