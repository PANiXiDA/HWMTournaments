using DTOs.Requests;

namespace UI.Client.Services.Interfaces;

public interface IAuthService
{
    Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task LogoutAsync(CancellationToken cancellationToken = default);
}
