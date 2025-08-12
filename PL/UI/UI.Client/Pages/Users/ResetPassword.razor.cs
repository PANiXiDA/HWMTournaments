using System.Net.Http.Json;

using Common.Helpers;

using DTOs.Requests;

using Microsoft.AspNetCore.Components;

namespace UI.Client.Pages.Users;

public partial class ResetPassword : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private ILogger<ResetPassword> Logger { get; set; } = default!;

    [Parameter, SupplyParameterFromQuery] public string? Email { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? Token { get; set; }

    protected bool IsLoading { get; set; } = true;
    protected bool IsSubmitting { get; set; }
    protected bool IsSuccess { get; set; }

    protected string ErrorMessage { get; set; } = string.Empty;

    protected ResetPasswordRequest Model { get; set; } = new();

    protected override Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
        {
            IsLoading = false;
            IsSuccess = false;
            ErrorMessage = "Отсутствуют параметры (email/token). Запросите новую ссылку на сброс пароля.";
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

        try
        {
            IsSubmitting = true;
            ErrorMessage = string.Empty;

            var url = "api/v1/users/reset-password";
            Logger.LogInformation("Сброс пароля для email={Email}", Model.Email);

            using var request = new HttpRequestMessage(new HttpMethod("PATCH"), url)
            {
                Content = JsonContent.Create(Model)
            };

            var response = await Http.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var detail = ApiErrorHelper.TryExtractDetail(json);
                ErrorMessage = string.IsNullOrWhiteSpace(detail)
                    ? $"Не удалось сменить пароль (код {(int)response.StatusCode}). Проверьте ссылку и попробуйте ещё раз."
                    : detail;
                IsSuccess = false;
                return;
            }

            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Исключение при сбросе пароля");
            ErrorMessage = "Произошла непредвиденная ошибка. Попробуйте позже.";
            IsSuccess = false;
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    protected void GoHome() => Nav.NavigateTo("/");
}
