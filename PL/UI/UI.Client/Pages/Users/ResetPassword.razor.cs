using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Users;

public partial class ResetPassword : ComponentBase
{
    [Inject] private ILogger<ResetPassword> Logger { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IUsersService UsersService { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public string? Email { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Token { get; set; }

    protected bool IsLoading { get; set; } = true;
    protected bool IsSubmitting { get; set; }
    protected bool IsSuccess { get; set; }

    protected ResetPasswordRequest Model { get; set; } = new();

    protected override Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            IsLoading = false;
            IsSuccess = false;
            return Task.CompletedTask;
        }

        Model = new ResetPasswordRequest
        {
            Email = Email!,
            Token = Token!
        };
        IsLoading = false;

        return Task.CompletedTask;
    }

    protected async Task SubmitAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        IsSubmitting = true;

        await UsersService.ResetPasswordAsync(Model);

        IsSuccess = true;
        IsSubmitting = false;
    }
}
