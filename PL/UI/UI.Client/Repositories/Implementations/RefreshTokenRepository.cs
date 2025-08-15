using Microsoft.JSInterop;

using UI.Client.Repositories.Implementations.Core;
using UI.Client.Repositories.Interfaces;

namespace UI.Client.Repositories.Implementations;

public sealed class RefreshTokenRepository(IJSRuntime js) : LocalStorageRepository<string>(js), IRefreshTokenRepository
{
    protected override string StorageKey => "auth.refresh.token";
}
