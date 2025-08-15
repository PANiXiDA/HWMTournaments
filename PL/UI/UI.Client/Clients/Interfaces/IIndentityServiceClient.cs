using DTOs.Requests;

namespace UI.Client.Clients.Interfaces;

public interface IIndentityServiceClient
{
    Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task RefreshAsync(CancellationToken cancellationToken = default);
}
