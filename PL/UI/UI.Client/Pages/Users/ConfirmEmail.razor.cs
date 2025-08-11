using Common.Helpers;

using Microsoft.AspNetCore.Components;

namespace UI.Client.Pages.Users;

public partial class ConfirmEmail : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private ILogger<ConfirmEmail> Logger { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public string? Email { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Token { get; set; }

    protected bool IsLoading { get; set; } = true;
    protected bool IsSuccess { get; set; }
    protected string ErrorMessage { get; set; } = "Проверьте ссылку из письма и повторите попытку.";

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            IsLoading = false;
            IsSuccess = false;
            ErrorMessage = "Отсутствуют параметры подтверждения (email/token).";
            return;
        }

        try
        {
            var url = $"api/v1/users/confirm-email?email={Uri.EscapeDataString(Email!)}&token={Uri.EscapeDataString(Token!)}";
            Logger.LogInformation("Подтверждаем e-mail={Email}", Email);

            var response = await Http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                IsSuccess = true;
            }
            else
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                Logger.LogWarning("Ошибка подтверждения e-mail: {Error}", errorJson);

                var detail = ApiErrorHelper.TryExtractDetail(errorJson);
                ErrorMessage = string.IsNullOrWhiteSpace(detail)
                    ? $"Ошибка подтверждения (код {(int)response.StatusCode})."
                    : detail;

                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Исключение при подтверждении e-mail");
            ErrorMessage = "Произошла непредвиденная ошибка. Попробуйте позже.";
            IsSuccess = false;
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected void GoLogin() => Nav.NavigateTo("/login");
    protected void GoHome() => Nav.NavigateTo("/");
}