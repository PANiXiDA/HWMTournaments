using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Auth;

public partial class Registration
{
    [Inject] private ILogger<Registration> Logger { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IUsersService UsersService { get; set; } = default!;

    private readonly RegistrationRequest _registrationRequest = new();
    private bool _isSubmitting;

    private async Task HandleRegistration()
    {
        if (_isSubmitting)
        {
            return;
        }
        _isSubmitting = true;

        await UsersService.RegisterAsync(_registrationRequest);

        _isSubmitting = false;
    }
}
