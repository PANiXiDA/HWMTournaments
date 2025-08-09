using System.Net.Http.Headers;
using System.Text.Json;

using Common.Constants;

namespace UI.Client.Services;

public sealed class AuthService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly ILogger<AuthService> _logger;
    private readonly TokenService _tokenService;

    private readonly string _clientSecret;

    public AuthService(
        IHttpClientFactory httpFactory,
        ILogger<AuthService> logger,
        IConfiguration configuration,
        TokenService tokenService)
    {
        _httpFactory = httpFactory;
        _logger = logger;
        _tokenService = tokenService;
        _clientSecret = configuration.GetValue<string>("IdentityServerSettings:ClientSecret") ?? string.Empty;
    }

    public async Task<(bool ok, string? error)> LoginAsync(string login, string password)
    {
        var client = _httpFactory.CreateClient(ClientsConstants.IdentityService);

        var form = new Dictionary<string, string?>
        {
            ["grant_type"] = IdentityServiceConstants.GrantTypes.Login,
            ["scope"] = IdentityServiceConstants.DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName,
            ["client_id"] = IdentityServiceConstants.Clients.Blazor,
            ["client_secret"] = _clientSecret,
            [IdentityServiceConstants.RequestParameters.Login] = login,
            [IdentityServiceConstants.RequestParameters.Password] = password
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "connect/token")
        {
            Content = new FormUrlEncodedContent(form!)
        };
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _logger.LogInformation("Отправка запроса токена в IdentityService для login={Login}", login);

        using var response = await client.SendAsync(request);
        var json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Не удалось получить токен. Код={StatusCode}, Ответ={Body}", (int)response.StatusCode, json);
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("error_description", out var desc))
            {
                return (false, desc.GetString());
            }
        }

        try
        {
            var token = JsonDocument.Parse(json).RootElement.GetProperty("access_token").GetString();
            if (string.IsNullOrWhiteSpace(token))
            {
                return (false, "Токен не получен");
            }
            await _tokenService.SetTokenAsync(token!);
            _logger.LogInformation("Токен успешно сохранён в localStorage");

            return (true, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка парсинга ответа токена");
            return (false, "Неверный формат ответа");
        }
    }
}