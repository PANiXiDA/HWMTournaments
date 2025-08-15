namespace UI.Client.Repositories.Interfaces.Core;

public interface ILocalStorageRepository<T>
{
    Task<T?> GetAsync(CancellationToken cancellationToken = default);
    Task SetAsync(T value, CancellationToken cancellationToken = default);
    Task RemoveAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
}
