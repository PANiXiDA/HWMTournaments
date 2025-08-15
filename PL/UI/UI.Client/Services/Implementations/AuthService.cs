using DTOs.Requests;

using UI.Client.Clients.Interfaces;
using UI.Client.Repositories.Interfaces;
using UI.Client.Services.Interfaces;

namespace UI.Client.Services.Implementations;

public sealed class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;

    private readonly IIndentityServiceClient _indentityServiceClient;

    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        ILogger<AuthService> logger,
        IIndentityServiceClient indentityServiceClient,
        IAccessTokenRepository accessTokenRepository,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _logger = logger;

        _indentityServiceClient = indentityServiceClient;

        _accessTokenRepository = accessTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        await _indentityServiceClient.LoginAsync(request, cancellationToken);
    }

    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        await _accessTokenRepository.RemoveAsync(cancellationToken);
        await _refreshTokenRepository.RemoveAsync(cancellationToken);
    }
}
