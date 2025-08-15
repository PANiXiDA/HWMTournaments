using Common.SearchParams;
using Common.SearchParams.Core;

using DTOs.Models;

using Microsoft.AspNetCore.Components;

using UI.Client.Components;
using UI.Client.Services.Interfaces;

namespace UI.Client.Pages.Tournaments;

public partial class Tournaments : ComponentBase
{
    [Inject] private NavigationManager Nav { get; set; } = default!;
    [Inject] private ITournamentsService TournamentsService { get; set; } = default!;

    private TournamentsSearchParams _searchParams = new() { Page = 1, ObjectsCount = 10 };
    private SearchResult<TournamentDTO>? _searchResult;
    private Pagination<TournamentDTO, TournamentsSearchParams>? _pager;

    protected async Task<SearchResult<TournamentDTO>?> LoadTournamentsAsync(TournamentsSearchParams searchParams)
    {
        return await TournamentsService.GetAsync(searchParams);
    }

    protected void OnCreateTournament()
    {
        Nav.NavigateTo("/tournaments/create");
    }

    protected async Task DeleteAsync(int id)
    {
        await TournamentsService.DeleteAsync(id);
        if (_pager is not null)
        {
            await _pager.ApplyFiltersAsync(resetPageToFirst: false);
        }
    }
}