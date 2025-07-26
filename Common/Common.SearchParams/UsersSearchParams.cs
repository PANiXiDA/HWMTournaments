using Common.SearchParams.Core;

namespace Common.SearchParams;

public sealed class UsersSearchParams : BaseSearchParams
{
    public UsersSearchParams() : base() { }

    public UsersSearchParams(string? searchQuery = null, int startIndex = 0, int? objectsCount = null) : base(startIndex, objectsCount)
    {
        SearchQuery = searchQuery;
    }
}