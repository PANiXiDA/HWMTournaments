using Common.Constants;

using UI.Client.Clients.Interfaces;
using UI.Client.Services;

namespace UI.Client.Clients.Implementations;

public sealed class IndentityServiceClient : IIndentityServiceClient
{
    private readonly ILogger<ServerAPIClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly NotificationService _notificationService;

    public IndentityServiceClient(
        ILogger<ServerAPIClient> logger,
        IHttpClientFactory httpClientFactory,
        NotificationService notificationService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _notificationService = notificationService;
    }

    private HttpClient Create()
    {
        return _httpClientFactory.CreateClient(ClientsConstants.IdentityServiceClient);
    }
}
