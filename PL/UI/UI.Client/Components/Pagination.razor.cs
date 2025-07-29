using System.Globalization;
using System.Net.Http.Json;

using Common.SearchParams.Core;

using DTOs.Core;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace UI.Client.Components;

public partial class Pagination<TDTO, TSearchParams> where TSearchParams : BaseSearchParams, new()
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private HttpClient Http { get; set; } = default!;

    [Parameter] public TSearchParams SearchParams { get; set; } = new();
    [Parameter] public EventCallback<TSearchParams> SearchParamsChanged { get; set; }

    [Parameter] public SearchResult<TDTO> SearchResult { get; set; } = new();
    [Parameter] public EventCallback<SearchResult<TDTO>> SearchResultChanged { get; set; }

    [Parameter] public string ApiUrl { get; set; } = default!;

    [Parameter] public Func<TSearchParams, IReadOnlyDictionary<string, string?>>? BuildQuery { get; set; }

    private int TotalPages => Math.Max(1, (int)Math.Ceiling((double)SearchResult.Total / Math.Max(1, SearchResult.RequestedObjectsCount)));
    private bool IsFirst => SearchResult.RequestedPage <= 1;
    private bool IsLast => SearchResult.RequestedPage >= TotalPages;

    private IEnumerable<int?> VisiblePages
    {
        get
        {
            var pages = new List<int?>();

            if (TotalPages <= 7)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
                return pages;
            }

            pages.Add(1);
            pages.Add(2);

            var currentPage = SearchResult.RequestedPage;
            var startPage = Math.Max(3, currentPage - 2);
            var endPage = Math.Min(TotalPages - 2, currentPage + 2);

            if (startPage > 3)
            {
                pages.Add(null);
            }
            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(i);
            }
            if (endPage < TotalPages - 2)
            {
                pages.Add(null);
            }

            pages.Add(TotalPages - 1);
            pages.Add(TotalPages);

            return pages;
        }
    }

    protected override void OnParametersSet()
    {
        SearchResult ??= new SearchResult<TDTO>();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ReadParamsFromQuery();

            await LoadAsync();
            StateHasChanged();
        }
    }

    public async Task ApplyFiltersAsync(bool resetPageToFirst = false)
    {
        if (resetPageToFirst)
        {
            SearchParams.Page = 1;
        }
        await SearchParamsChanged.InvokeAsync(SearchParams);

        UpdateQueryFromParams();
        await LoadAsync();
    }

    private async Task Go(int page)
    {
        page = Math.Clamp(page, 1, Math.Max(1, TotalPages));
        if (SearchParams.Page == page)
        {
            return;
        }

        SearchParams.Page = page;
        await SearchParamsChanged.InvokeAsync(SearchParams);

        UpdateQueryFromParams();
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var query = BuildQuery?.Invoke(SearchParams) ?? BuildQueryFromParams(SearchParams);
        var url = QueryHelpers.AddQueryString(ApiUrl, query);

        var response = await Http.GetFromJsonAsync<RestApiResponse<SearchResult<TDTO>>>(url);
        SearchResult = response?.Payload ?? new SearchResult<TDTO>();

        await SearchResultChanged.InvokeAsync(SearchResult);
    }

    private void UpdateQueryFromParams()
    {
        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        var baseUri = uri.GetLeftPart(UriPartial.Path);
        var query = BuildQuery?.Invoke(SearchParams) ?? BuildQueryFromParams(SearchParams);

        var newUri = QueryHelpers.AddQueryString(baseUri, query);
        Nav.NavigateTo(newUri);
    }

    private void ReadParamsFromQuery()
    {
        var uri = Nav.ToAbsoluteUri(Nav.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        var type = typeof(TSearchParams);
        foreach (var property in type.GetProperties().Where(item => item.CanWrite))
        {
            if (!query.TryGetValue(property.Name, out var value))
            {
                continue;
            }
            var s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                continue;
            }

            object? parsed = property.PropertyType switch
            {
                var t when t == typeof(string) => s,
                var t when t == typeof(int) || t == typeof(int?) => int.TryParse(s, out var i) ? (object?)i : null,
                var t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(s, out var b) ? (object?)b : null,
                var t when t.IsEnum => Enum.Parse(t, s, ignoreCase: true),
                var t when Nullable.GetUnderlyingType(t)?.IsEnum == true => Enum.Parse(Nullable.GetUnderlyingType(t)!, s, ignoreCase: true),
                var t when t == typeof(DateTime) || t == typeof(DateTime?) => DateTime.TryParse(s, out var dt) ? (object?)dt : null,
                _ => s
            };

            if (parsed is not null)
            {
                property.SetValue(SearchParams, parsed);
            }
        }
    }

    private static Dictionary<string, string?> BuildQueryFromParams(TSearchParams searchParams)
    {
        var dictionary = new Dictionary<string, string?>();

        foreach (var property in typeof(TSearchParams).GetProperties().Where(item => item.CanRead))
        {
            var val = property.GetValue(searchParams);
            if (val is null)
            {
                continue;
            }

            string? s = val switch
            {
                DateTime dt => dt.ToString("o"),
                Enum e => e.ToString(),
                bool b => b ? "true" : "false",
                _ => Convert.ToString(val, CultureInfo.InvariantCulture)
            };

            if (!string.IsNullOrWhiteSpace(s))
            {
                dictionary[property.Name] = s;
            }
        }

        if (!dictionary.ContainsKey(nameof(BaseSearchParams.ObjectsCount)) && searchParams.ObjectsCount.HasValue)
        {
            dictionary[nameof(BaseSearchParams.ObjectsCount)] = searchParams.ObjectsCount!.Value.ToString();
        }

        if (!dictionary.ContainsKey(nameof(BaseSearchParams.Page)))
        {
            dictionary[nameof(BaseSearchParams.Page)] = (searchParams.Page > 0 ? searchParams.Page : 1).ToString();
        }

        return dictionary;
    }
}
