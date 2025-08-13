using Common.ConvertParams;
using Common.Helpers;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Core;
using DTOs.Models;

using UI.Client.Clients.Interfaces;
using UI.Client.Services.Interfaces;

namespace UI.Client.Services.Implementations;

public sealed class TournamentsService : ITournamentsService
{
    private readonly ILogger<TournamentsService> _logger;

    private readonly IServerAPIClient _serverAPIClient;

    private const string BaseEndpoint = "api/v1/tournaments";

    public TournamentsService(
        ILogger<TournamentsService> logger,
        IServerAPIClient serverAPIClient)
    {
        _logger = logger;

        _serverAPIClient = serverAPIClient;
    }

    public async Task<TournamentDTO?> GetAsync(int id, TournamentsConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var url = QueryStringHelper.Append($"{BaseEndpoint}/{id}", convertParams);
        return await _serverAPIClient.GetAsync<TournamentDTO>(url, "Данные о турнире успешно получены.", "Не удалось получить турнир.", cancellationToken);
    }

    public async Task<SearchResult<TournamentDTO>?> GetAsync(TournamentsSearchParams searchParams, TournamentsConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var url = QueryStringHelper.AppendMany($"{BaseEndpoint}/get-by-filter", searchParams, convertParams);
        return await _serverAPIClient.GetAsync<SearchResult<TournamentDTO>>(url, "Список турниров успешно получен.", "Не удалось получить список турниров.", cancellationToken);
    }

    public async Task<int?> CreateAsync(TournamentDTO dto, CancellationToken cancellationToken = default)
    {
        return await _serverAPIClient.PostAsync<TournamentDTO, int?>(BaseEndpoint, dto, "Турнир успешно создан.", "Не удалось создать турнир.", cancellationToken);
    }

    public async Task UpdateAsync(int id, TournamentDTO dto, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PutAsync<TournamentDTO, NoContent>($"{BaseEndpoint}/{id}", dto, "Турнир успешно сохранен.", "Не удалось сохранить турнир.", cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.DeleteAsync<NoContent>($"{BaseEndpoint}/{id}", "Турнир успешно удален.", "Не удалось удалить турнир.", cancellationToken);
    }
}
