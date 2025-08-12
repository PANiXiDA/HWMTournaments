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

    private SendEmailConfirmationLinkRequest _sendEmailConfirmationLinkRequest = new();
    private bool _emailConfirmationModalOpen;

    private SendPasswordResetLinkRequest _sendPasswordResetLinkRequest = new();
    private bool _passwordResetModalOpen;

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
        _emailConfirmationModalOpen = true;
    }

    private void ForgotPassword()
    {
        _passwordResetModalOpen = true;
    }

    private async Task OnSendEmailConfirmationLinkSuccess()
    {
        await Notifications.NotifyAsync("Письмо для подтверждения отправлено. Проверьте почту.");
    }

    private async Task OnSendPasswordResetLinkSuccess()
    {
        await Notifications.NotifyAsync("Письмо для сброса пароля отправлено. Проверьте почту.");
    }
}
