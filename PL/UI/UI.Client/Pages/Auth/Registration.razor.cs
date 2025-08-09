using System.Net.Http.Json;

using Common.Helpers;

using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services;

namespace UI.Client.Pages.Auth;

public partial class Registration
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ILogger<Registration> Logger { get; set; } = default!;
    [Inject] private NotificationService Notifications { get; set; } = default!;

    private readonly RegistrationRequest _registrationRequest = new();

    private async Task HandleRegistration()
    {
        try
        {
            Logger.LogInformation("Отправка запроса регистрации для Email={Email}", _registrationRequest.Email);

            var response = await Http.PostAsJsonAsync("api/v1/auth/registration", _registrationRequest);

            if (response.IsSuccessStatusCode)
            {
                Logger.LogInformation("Регистрация успешна");
                await Notifications.NotifyAsync("Регистрация прошла успешно!");
                return;
            }

            var errorJson = await response.Content.ReadAsStringAsync();
            Logger.LogWarning("Ошибка регистрации: {Error}", errorJson);

            var detail = ApiErrorHelper.TryExtractDetail(errorJson);
            var message = string.IsNullOrWhiteSpace(detail)
                ? "Не удалось зарегистрироваться. Проверьте данные и попробуйте ещё раз."
                : $"Не удалось зарегистрироваться: {detail}";

            await Notifications.NotifyAsync(message, isError: true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Исключение при регистрации");
            await Notifications.NotifyAsync($"Произошла ошибка: {ex.Message}", isError: true);
        }
    }
}
