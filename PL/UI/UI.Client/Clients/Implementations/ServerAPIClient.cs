using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using Common.Constants;

using DTOs.Core;

using UI.Client.Clients.Interfaces;
using UI.Client.Services;

namespace UI.Client.Clients.Implementations;

public sealed class ServerAPIClient : IServerAPIClient
{
    private readonly ILogger<ServerAPIClient> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly NotificationService _notificationService;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ServerAPIClient(
        ILogger<ServerAPIClient> logger,
        IHttpClientFactory httpFactory,
        NotificationService notifications)
    {
        _logger = logger;
        _httpClientFactory = httpFactory;

        _notificationService = notifications;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string url, string? successMessage = null, string? failMessage = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(BuildJson(HttpMethod.Get, url), successMessage ?? "Данные успешно получены.", failMessage ?? "Не удалось загрузить данные.", cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, string? successMessage = null, string? failMessage = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(BuildJson(HttpMethod.Post, url, request), successMessage ?? "Операция успешно выполнена.", failMessage ?? "Не удалось выполнить операцию.", cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest request, string? successMessage = null, string? failMessage = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(BuildJson(HttpMethod.Put, url, request), successMessage ?? "Изменения успешно сохранились.", failMessage ?? "Не удалось сохранить изменения.", cancellationToken);
    }

    public async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest request, string? successMessage = null, string? failMessage = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<TResponse>(BuildJson(HttpMethod.Patch, url, request), successMessage ?? "Изменения успешно сохранились.", failMessage ?? "Не удалось сохранить изменения.", cancellationToken);
    }

    public async Task<T?> DeleteAsync<T>(string url, string? successMessage = null, string? failMessage = null, CancellationToken cancellationToken = default)
    {
        return await SendAsync<T>(BuildJson(HttpMethod.Delete, url), successMessage ?? "Удаление прошло успешно.", failMessage ?? "Не удалось удалить.", cancellationToken);
    }

    private HttpClient Create()
    {
        return _httpClientFactory.CreateClient(ClientsConstants.ServerAPIClient);
    }

    private static HttpRequestMessage BuildJson(HttpMethod method, string url, object? payload = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        if (payload is not null)
        {
            var json = JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        return request;
    }

    private async Task<TResponse?> SendAsync<TResponse>(HttpRequestMessage request, string successMessage, string failMessage, CancellationToken cancellationToken)
    {
        using var client = Create();
        using var response = await client.SendAsync(request, cancellationToken);
        var status = response.StatusCode;
        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            if (status == HttpStatusCode.NoContent)
            {
                return default;
            }

            try
            {
                var restApiResponse = JsonSerializer.Deserialize<RestApiResponse<TResponse>>(json, JsonSerializerOptions);

                if (restApiResponse == null)
                {
                    return default;
                }
                if (restApiResponse is { Failure: null })
                {
                    if (request.Method != HttpMethod.Get)
                    {
                        await NotifySuccessAsync(successMessage);
                    }
                    return restApiResponse.Payload;
                }

                var detail = TryExtractErrorDetail(json) ?? "Неизвестная ошибка ответа.";
                _logger.LogError("2xx, но Failure: {Detail}. Raw: {Json}", detail, json);
                await NotifyErrorAsync(failMessage, detail);

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Deserialize<RestApiResponse<{Type}>> failed. Raw: {Json}", typeof(TResponse).Name, json);
                await NotifyErrorAsync(failMessage, "Ошибка обработки ответа сервера.");

                return default;
            }
        }
        else
        {
            var detail = TryExtractErrorDetail(json) ?? $"Ошибка запроса: {(int)status}.";
            _logger.LogError("{Method} {Url} failed: {Status} {Detail} Raw: {Json}", request.Method, request.RequestUri, status, detail, json);
            await NotifyErrorAsync(failMessage, detail);

            return default;
        }
    }

    private static string? TryExtractErrorDetail(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            var response = JsonSerializer.Deserialize<RestApiResponse<object>>(json, JsonSerializerOptions);
            if (response?.Failure?.Errors is { Count: > 0 })
            {
                var firstError = response.Failure.Errors.Values.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(firstError))
                {
                    return null;
                }

                var match = Regex.Match(firstError, @"Detail\s*=\s*""([^""]+)""");
                return match.Success ? match.Groups[1].Value : firstError;
            }

            return null;
        }
        catch { }

        return null;
    }

    private async Task NotifySuccessAsync(string message)
    {
        await _notificationService.NotifyAsync(message);
    }

    private async Task NotifyErrorAsync(string baseText, string detail)
    {
        var message = string.IsNullOrWhiteSpace(detail) ? baseText : $"{baseText} Детали: {detail}";
        await _notificationService.NotifyAsync(message, isError: true);
    }
}
