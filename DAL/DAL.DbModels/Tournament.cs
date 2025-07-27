using Common.Enums;

using DAL.DbModels.Core;

namespace DAL.DbModels;

public sealed class Tournament : BaseDbModel<int>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public TournamentFormat Format { get; set; }
    public TournamentGridType GridType { get; set; }
    public bool FactionsBan { get; set; }
    public int? MinimumElo { get; set; }
    public int? MaximumElo { get; set; }
}
