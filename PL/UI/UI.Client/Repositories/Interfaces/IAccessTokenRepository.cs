using UI.Client.Repositories.Interfaces.Core;

namespace UI.Client.Repositories.Interfaces;

public interface IAccessTokenRepository : ILocalStorageRepository<string>
{
}
