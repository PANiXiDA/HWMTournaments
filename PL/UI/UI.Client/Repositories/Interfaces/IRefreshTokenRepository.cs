using UI.Client.Repositories.Interfaces.Core;

namespace UI.Client.Repositories.Interfaces;

public interface IRefreshTokenRepository : ILocalStorageRepository<string>
{
}
