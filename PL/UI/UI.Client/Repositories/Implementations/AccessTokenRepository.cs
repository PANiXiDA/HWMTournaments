using Microsoft.JSInterop;

using UI.Client.Repositories.Implementations.Core;
using UI.Client.Repositories.Interfaces;

namespace UI.Client.Repositories.Implementations;

public sealed class AccessTokenRepository(IJSRuntime js) : LocalStorageRepository<string>(js), IAccessTokenRepository
{
    protected override string StorageKey => "auth.access.token";
}
