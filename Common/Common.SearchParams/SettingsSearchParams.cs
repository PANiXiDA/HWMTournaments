using Common.SearchParams.Core;

namespace Common.SearchParams;

public sealed class SettingsSearchParams : BaseSearchParams
{
    public SettingsSearchParams() : base() { }

    public SettingsSearchParams(int page = 1, int? objectsCount = null) : base(page, objectsCount)
    {
    }
}
