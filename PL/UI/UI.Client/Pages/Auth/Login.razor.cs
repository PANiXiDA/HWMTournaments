using Microsoft.AspNetCore.Components;
using UI.Client.Services;
using DTOs.Requests;

namespace UI.Client.Pages.Auth;

public partial class Login
{
    [Inject] private ILogger<Login> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private NotificationService Notifications { get; set; } = default!;
    [Inject] private AuthService Auth { get; set; } = default!;

    private readonly LoginRequest _loginRequest = new();

    private async Task HandleLogin()
    {
        try
        {
            Logger.LogInformation("Авторизация: Name={Name}", _loginRequest.Login);

            var (ok, error) = await Auth.LoginAsync(_loginRequest.Login, _loginRequest.Password);
            if (ok)
            {
                await Notifications.NotifyAsync("Вы успешно вошли!");
                // TODO: при желании — редирект на главную/профиль
                return;
            }

            await Notifications.NotifyAsync($"Ошибка входа: {error}", isError: true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Исключение при авторизации");
            await Notifications.NotifyAsync($"Произошла ошибка: {ex.Message}", isError: true);
        }
    }

    private void ConfirmEmail() => Notifications.NotifyAsync("Переход на подтверждение почты...");
    private void ForgotPassword() => Notifications.NotifyAsync("Переход на восстановление пароля...");
}
