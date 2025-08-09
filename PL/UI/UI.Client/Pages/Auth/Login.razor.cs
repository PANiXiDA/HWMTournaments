using System.Net.Http.Json;

using Common.Helpers;

using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services;

namespace UI.Client.Pages.Auth;

public partial class Login
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Login> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private NotificationService Notifications { get; set; } = default!;

    private readonly LoginRequest _loginRequest = new();

    private async Task HandleLogin()
    {
        try
        {
            Logger.LogInformation("Отправка запроса авторизации для Name={Name}", _loginRequest.Name);

            var response = await Http.PostAsJsonAsync("api/v1/auth/login", _loginRequest);

            if (response.IsSuccessStatusCode)
            {
                Logger.LogInformation("Авторизация успешна");
                await Notifications.NotifyAsync("Вы успешно вошли в систему!");
                return;
            }

            var errorJson = await response.Content.ReadAsStringAsync();
            Logger.LogWarning("Ошибка авторизации: {Error}", errorJson);

            var detail = ApiErrorHelper.TryExtractDetail(errorJson);
            var message = string.IsNullOrWhiteSpace(detail)
                ? "Не удалось авторизоваться. Проверьте данные и попробуйте ещё раз."
                : $"Ошибка входа: {detail}";

            await Notifications.NotifyAsync(message, isError: true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Исключение при авторизации");
            await Notifications.NotifyAsync($"Произошла ошибка: {ex.Message}", isError: true);
        }
    }

    private void ConfirmEmail()
    {
        Notifications.NotifyAsync("Переход на подтверждение почты...");
    }

    private void ForgotPassword()
    {
        Notifications.NotifyAsync("Переход на восстановление пароля...");
    }
}
