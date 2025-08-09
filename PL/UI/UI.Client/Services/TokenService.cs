using Microsoft.JSInterop;

namespace UI.Client.Services;

public sealed class TokenService
{
    private readonly IJSRuntime _js;
    private const string Key = "auth_token";

    public TokenService(IJSRuntime js) => _js = js;

    public ValueTask SetTokenAsync(string token) => _js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask<string?> GetTokenAsync() => _js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask ClearAsync() => _js.InvokeVoidAsync("localStorage.removeItem", Key);
}
