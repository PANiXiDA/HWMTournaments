using System.Net.Http.Headers;
using System.Text.Json;

using Common.Constants;

using DTOs.Requests;
using DTOs.Responses;

using UI.Client.Clients.Interfaces;
using UI.Client.Repositories.Interfaces;
using UI.Client.Services.Interfaces;

namespace UI.Client.Clients.Implementations;

public sealed class IndentityServiceClient : IIndentityServiceClient
{
    private readonly ILogger<ServerAPIClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly IAccessTokenRepository _accessStorage;
    private readonly IRefreshTokenRepository _refreshStorage;

    private readonly INotificationsService _notificationService;

    private readonly string _clientSecret;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public IndentityServiceClient(
        ILogger<ServerAPIClient> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IAccessTokenRepository accessStorage,
        IRefreshTokenRepository refreshStorage,
        INotificationsService notificationService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;

        _accessStorage = accessStorage;
        _refreshStorage = refreshStorage;

        _notificationService = notificationService;

        _clientSecret = configuration.GetValue<string>("IdentityServerSettings:ClientSecret") ?? string.Empty;
    }

    public async Task LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var form = new Dictionary<string, string?>
        {
            ["grant_type"] = IdentityServiceConstants.GrantTypes.Login,
            ["scope"] = $"{IdentityServiceConstants.DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName} {IdentityServiceConstants.DevTemplateAspNetCoreAPI.OfflineAccess}",
            ["client_id"] = IdentityServiceConstants.Clients.Blazor,
            ["client_secret"] = _clientSecret,
            [IdentityServiceConstants.RequestParameters.Login] = request.Login,
            [IdentityServiceConstants.RequestParameters.Password] = request.Password
        };

        await SendAsync(form, onSuccessMessage: "Вы успешно вошли!", onFailDefault: "Не удалось войти.", sendNotification: true, cancellationToken);
    }

    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        var refreshToken = await _refreshStorage.GetAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var form = new Dictionary<string, string?>
        {
            ["grant_type"] = IdentityServiceConstants.GrantTypes.Refresh,
            ["scope"] = $"{IdentityServiceConstants.DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName} {IdentityServiceConstants.DevTemplateAspNetCoreAPI.OfflineAccess}",
            ["client_id"] = IdentityServiceConstants.Clients.Blazor,
            ["client_secret"] = _clientSecret,
            ["refresh_token"] = refreshToken
        };

        await SendAsync(form, onSuccessMessage: string.Empty, onFailDefault: string.Empty, sendNotification: false, cancellationToken);
    }

    private HttpClient Create()
    {
        return _httpClientFactory.CreateClient(ClientsConstants.IdentityServiceClient);
    }

    private async Task SendAsync(Dictionary<string, string?> form, string onSuccessMessage, string onFailDefault, bool sendNotification, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "connect/token")
        {
            Content = new FormUrlEncodedContent(form!)
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var client = Create();

        using var response = await client.SendAsync(request, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Token request failed. Code={Code}, Body={Body}", (int)response.StatusCode, json);
            try
            {
                using var document = JsonDocument.Parse(json);
                if (document.RootElement.TryGetProperty("error_description", out var description) && !string.IsNullOrWhiteSpace(description.GetString()))
                {
                    if (sendNotification)
                    {
                        await _notificationService.NotifyAsync(description.GetString()!, isError: true);
                    }
                    return;
                }
            }
            catch { }

            if (sendNotification)
            {
                await _notificationService.NotifyAsync(onFailDefault, isError: true);
            }
            return;
        }

        try
        {
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, JsonOptions);
            if (tokenResponse is null || string.IsNullOrWhiteSpace(tokenResponse.AccessToken) && sendNotification)
            {
                await _notificationService.NotifyAsync(onFailDefault, isError: true);
                return;
            }

            await _accessStorage.SetAsync(tokenResponse.AccessToken!, cancellationToken);

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                await _refreshStorage.SetAsync(tokenResponse.RefreshToken!, cancellationToken);
            }

            if (sendNotification)
            {
                await _notificationService.NotifyAsync(onSuccessMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка парсинга ответа токена");
            if (sendNotification)
            {
                await _notificationService.NotifyAsync(onFailDefault, isError: true);
            }
        }
    }
}
