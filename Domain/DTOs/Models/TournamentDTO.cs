using Common.Enums;
using DTOs.Models.Core;

namespace DTOs.Models;

public sealed class TournamentDTO : BaseDTO<int>
{
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime StartDate { get; set; } = default!;
    public TournamentFormat Format { get; set; } = default!;
    public TournamentGridType GridType { get; set; } = default!;
    public bool FactionsBan { get; set; } = default!;
    public int? MinimumElo { get; set; } = default!;
    public int? MaximumElo { get; set; } = default!;
}
