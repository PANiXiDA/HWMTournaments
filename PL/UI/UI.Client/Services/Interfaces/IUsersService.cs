using Common.ConvertParams;
using Common.SearchParams;

using DTOs.Models;
using DTOs.Requests;

using UI.Client.Services.Interfaces.Core;

namespace UI.Client.Services.Interfaces;

public interface IUsersService : ICrudService<UserDTO, int, UsersSearchParams, UsersConvertParams>
{
    Task<int?> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
    Task SendEmailConfirmationLinkAsync(SendEmailConfirmationLinkRequest request, CancellationToken cancellationToken = default);
    Task SendPasswordResetLinkAsync(SendPasswordResetLinkRequest reques, CancellationToken cancellationToken = default);
    Task ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
