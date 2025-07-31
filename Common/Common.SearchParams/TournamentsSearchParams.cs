using Common.SearchParams.Core;
using Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace Common.SearchParams;

public sealed class TournamentsSearchParams : BaseSearchParams
{
    [Display(Name = "Формат")]
    public TournamentFormat? Format { get; set; }

    public TournamentsSearchParams() : base() { }

    public TournamentsSearchParams(string? searchQuery = null, int startIndex = 0, int? objectsCount = null) : base(startIndex, objectsCount)
    {
        SearchQuery = searchQuery;
    }
}