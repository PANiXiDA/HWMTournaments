using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Auth;

public partial class Login
{
    [Inject] private ILogger<Login> Logger { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private INotificationsService NotificationsService { get; set; } = default!;

    private readonly LoginRequest _loginRequest = new();

    private SendEmailConfirmationLinkRequest _sendEmailConfirmationLinkRequest = new();
    private bool _emailConfirmationModalOpen;

    private SendPasswordResetLinkRequest _sendPasswordResetLinkRequest = new();
    private bool _passwordResetModalOpen;

    private async Task HandleLogin()
    {
        await AuthService.LoginAsync(_loginRequest);
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
        await NotificationsService.NotifyAsync("Письмо для подтверждения отправлено. Проверьте почту.");
    }

    private async Task OnSendPasswordResetLinkSuccess()
    {
        await NotificationsService.NotifyAsync("Письмо для сброса пароля отправлено. Проверьте почту.");
    }
}
