using System.Text.Json;

using Microsoft.JSInterop;

using UI.Client.Repositories.Interfaces.Core;

namespace UI.Client.Repositories.Implementations.Core;

public abstract class LocalStorageRepository<T>(IJSRuntime js) : ILocalStorageRepository<T>
{
    protected readonly IJSRuntime Js = js;
    protected abstract string StorageKey { get; }

    protected virtual JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<T?> GetAsync(CancellationToken cancellationToken = default)
    {
        var raw = await Js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
        if (raw is null)
        {
            return default;
        }
        if (typeof(T) == typeof(string))
        {
            return (T)(object)raw;
        }

        return JsonSerializer.Deserialize<T>(raw, JsonOptions);
    }

    public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
    {
        var exists = await Js.InvokeAsync<string?>("localStorage.getItem", cancellationToken, StorageKey);
        return exists is not null;
    }

    public Task SetAsync(T value, CancellationToken cancellationToken = default)
    {
        var raw = typeof(T) == typeof(string)
            ? (string)(object)value!
            : JsonSerializer.Serialize(value, JsonOptions);

        return Js.InvokeVoidAsync("localStorage.setItem", cancellationToken, StorageKey, raw).AsTask();
    }

    public Task RemoveAsync(CancellationToken cancellationToken = default)
    {
        return Js.InvokeVoidAsync("localStorage.removeItem", cancellationToken, StorageKey).AsTask();
    }
}
