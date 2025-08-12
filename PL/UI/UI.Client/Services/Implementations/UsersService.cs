using Common.ConvertParams;
using Common.Helpers;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Core;
using DTOs.Models;

using UI.Client.Clients.Interfaces;
using UI.Client.Services.Interfaces;

namespace UI.Client.Services.Implementations;

public sealed class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _logger;

    private readonly IServerAPIClient _serverAPIClient;

    private const string BaseEndpoint = "api/v1/users";

    public UsersService(
        ILogger<UsersService> logger,
        IServerAPIClient serverAPIClient)
    {
        _logger = logger;

        _serverAPIClient = serverAPIClient;
    }

    public async Task<UserDTO?> GetAsync(int id, UsersConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var url = QueryStringHelper.Append($"{BaseEndpoint}/{id}", convertParams);
        return await _serverAPIClient.GetAsync<UserDTO>(url, "Не удалось загрузить пользователя.", cancellationToken);
    }

    public async Task<SearchResult<UserDTO>?> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var url = QueryStringHelper.AppendMany($"{BaseEndpoint}/get-by-filter", searchParams, convertParams);
        return await _serverAPIClient.GetAsync<SearchResult<UserDTO>>(url, "Не удалось загрузить список турниров.", cancellationToken);
    }

    public async Task<int?> CreateAsync(UserDTO dto, CancellationToken cancellationToken = default)
    {
        return await _serverAPIClient.PostAsync<UserDTO, int>(BaseEndpoint, dto, "Не удалось создать турнир.", cancellationToken);
    }

    public async Task UpdateAsync(int id, UserDTO dto, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PutAsync<UserDTO, NoContent>($"{BaseEndpoint}/{id}", dto, "Не удалось сохранить турнир.", cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.DeleteAsync<NoContent>($"{BaseEndpoint}/{id}", "Не удалось удалить турнир.", cancellationToken);
    }
}
