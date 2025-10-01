using Desafio.Api.Models;
using Desafio.Api.ViewModels;

namespace Desafio.Api.Services;

public interface IServicoTitulos
{
    Task<TituloResultado> CriarAsync(TituloEntrada entrada, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<TituloResultado>> ListarAsync(CancellationToken cancellationToken);
}

