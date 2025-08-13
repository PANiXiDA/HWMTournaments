using Common.ConvertParams;
using Common.Helpers;
using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Core;
using DTOs.Models;
using DTOs.Requests;

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
        return await _serverAPIClient.GetAsync<UserDTO>(url, "Данные о пользователе успешно получены.", "Не удалось получить пользователя.", cancellationToken);
    }

    public async Task<SearchResult<UserDTO>?> GetAsync(UsersSearchParams searchParams, UsersConvertParams? convertParams = null, CancellationToken cancellationToken = default)
    {
        var url = QueryStringHelper.AppendMany($"{BaseEndpoint}/get-by-filter", searchParams, convertParams);
        return await _serverAPIClient.GetAsync<SearchResult<UserDTO>>(url, "Список пользователей успешно получен.", "Не удалось получить список пользователей.", cancellationToken);
    }

    public async Task<int?> CreateAsync(UserDTO dto, CancellationToken cancellationToken = default)
    {
        return await _serverAPIClient.PostAsync<UserDTO, int?>(BaseEndpoint, dto, "Пользователь успешно создан.", "Не удалось создать пользователя.", cancellationToken);
    }

    public async Task<int?> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default)
    {
        return await _serverAPIClient.PostAsync<RegistrationRequest, int?>($"{BaseEndpoint}/registration", request, "Регистрация прошла успешно! Проверьте почту для подтверждения.", "Не удалось зарегистрироваться.", cancellationToken);
    }

    public async Task SendEmailConfirmationLinkAsync(SendEmailConfirmationLinkRequest request, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PostAsync<SendEmailConfirmationLinkRequest, NoContent>($"{BaseEndpoint}/send-email-confirmation-link", request, "Письмо для подтверждения успешно отправлено на указанную почту!", "Не удалось отправить письмо для подтверждения.", cancellationToken);
    }

    public async Task SendPasswordResetLinkAsync(SendPasswordResetLinkRequest request, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PostAsync<SendPasswordResetLinkRequest, NoContent>($"{BaseEndpoint}/send-password-reset-link", request, "Инструкция по сбросу пароля отправлена на указанную почту.", "Не удалось отправить письмо для сброса пароля.", cancellationToken);
    }

    public async Task UpdateAsync(int id, UserDTO dto, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PutAsync<UserDTO, NoContent>($"{BaseEndpoint}/{id}", dto, "Пользователь успешно сохранен.", "Не удалось сохранить пользователя.", cancellationToken);
    }

    public async Task ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PatchAsync<ConfirmEmailRequest, NoContent>($"{BaseEndpoint}/confirm-email", request, "e-mail успешно подтвержден!", "Не удалось подтвердить e-mail.", cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.PatchAsync<ResetPasswordRequest, NoContent>($"{BaseEndpoint}/reset-password", request, "Пароль успешно сброшен.", "Не удалось сбросить пароль.", cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _serverAPIClient.DeleteAsync<NoContent>($"{BaseEndpoint}/{id}", "Пользователь успешно удален.", "Не удалось удалить пользователя.", cancellationToken);
    }
}
