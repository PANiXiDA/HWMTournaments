using Common.Enums;
using Entities.Core;

namespace Entities;

public sealed class Tournament : BaseEntity<int>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public TournamentFormat Format { get; set; }
    public TournamentGridType GridType { get; set; }
    public bool FactionsBan { get; set; }
    public int? MinimumElo { get; set; }
    public int? MaximumElo { get; set; }

    public Tournament(
        int id,
        string name,
        string description,
        DateTime startDate,
        TournamentFormat format,
        TournamentGridType gridType,
        bool factionsBan,
        int? minimumElo,
        int? maximumElo) : base(id)
    {
        Name = name;
        Description = description;
        StartDate = startDate;
        Format = format;
        GridType = gridType;
        FactionsBan = factionsBan;
        MinimumElo = minimumElo;
        MaximumElo = maximumElo;
    }
}
