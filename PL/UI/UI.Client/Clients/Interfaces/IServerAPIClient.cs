namespace UI.Client.Clients.Interfaces;

public interface IServerAPIClient
{
    Task<TResponse?> GetAsync<TResponse>(string url, string? failMessage = null, CancellationToken cancellationToken = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, string? failMessage = null, CancellationToken cancellationToken = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest request, string? failMessage = null, CancellationToken cancellationToken = default);
    Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest request, string? failMessage = null, CancellationToken cancellationToken = default);
    Task<TResponse?> DeleteAsync<TResponse>(string url, string? failMessage = null, CancellationToken cancellationToken = default);
}
