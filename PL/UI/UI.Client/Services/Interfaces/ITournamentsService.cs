using Common.ConvertParams;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Models;

namespace UI.Client.Services.Interfaces;

public interface ITournamentsService
{
    Task<TournamentDTO?> GetAsync(int id, TournamentsConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<SearchResult<TournamentDTO>?> GetAsync(TournamentsSearchParams searchParams, TournamentsConvertParams? convertParams = null, CancellationToken cancellationToken = default);
    Task<int?> CreateAsync(TournamentDTO dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TournamentDTO dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
