using DTOs.Requests;

using Microsoft.AspNetCore.Components;

using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Users;

public partial class ConfirmEmail : ComponentBase
{
    [Inject] private ILogger<ConfirmEmail> Logger { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private IUsersService UsersService { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public string? Email { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Token { get; set; }

    protected bool IsLoading { get; set; } = true;
    protected bool IsSuccess { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            IsLoading = false;
            IsSuccess = false;
            return;
        }

        var request = new ConfirmEmailRequest
        {
            Email = Email!,
            Token = Token!
        };

        await UsersService.ConfirmEmailAsync(request);

        IsSuccess = true;
        IsLoading = false;
    }

    protected void GoLogin() => Nav.NavigateTo("/login");
    protected void GoHome() => Nav.NavigateTo("/");
}