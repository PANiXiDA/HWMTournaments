namespace Common.SearchParams.Core;

public sealed class SearchResult<T>
{
    public int Total { get; set; }
    public IList<T> Objects { get; set; } = new List<T>();
    public int RequestedPage { get; set; }
    public int RequestedStartIndex { get; set; }
    public int? RequestedObjectsCount { get; set; }

    public SearchResult() { }

    public SearchResult(int total, IEnumerable<T> objects, int requestedPage, int requestedStartIndex, int? requestedObjectsCount)
    {
        Total = total;
        Objects = objects.ToList();
        RequestedPage = requestedPage;
        RequestedObjectsCount = requestedObjectsCount;
        RequestedStartIndex = requestedStartIndex;
    }
}
