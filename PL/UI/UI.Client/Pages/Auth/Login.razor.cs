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

    private bool _emailConfirmationModalOpen;

    private readonly LoginRequest _loginRequest = new();
    private EmailConfirmationRequest _emailConfirmationRequest = new();

    private async Task HandleLogin()
    {
        try
        {
            Logger.LogInformation("Авторизация: Name={Name}", _loginRequest.Login);

            var (ok, error) = await Auth.LoginAsync(_loginRequest.Login, _loginRequest.Password);
            if (ok)
            {
                await Notifications.NotifyAsync("Вы успешно вошли!");
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

    private void ConfirmEmail()
    {
        _emailConfirmationRequest = new EmailConfirmationRequest
        {
            Email = _loginRequest.Login
        };
        _emailConfirmationModalOpen = true;
    }
    private async Task ForgotPassword()
    {
        await Notifications.NotifyAsync("Переход на восстановление пароля...");
    }

    private async Task OnConfirmSuccess()
    {
        await Notifications.NotifyAsync("Письмо для подтверждения отправлено. Проверьте почту.");
    }
}
