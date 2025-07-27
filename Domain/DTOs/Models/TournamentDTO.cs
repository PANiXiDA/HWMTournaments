using System;
using System.Collections.Generic;
using System.Linq;
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

    public static TournamentDTO FromEntity(Entities.Tournament obj)
    {
        return new TournamentDTO
        {
            Id = obj.Id,
            Name = obj.Name,
            Description = obj.Description,
            StartDate = obj.StartDate,
            Format = obj.Format,
            GridType = obj.GridType,
            FactionsBan = obj.FactionsBan,
            MinimumElo = obj.MinimumElo,
            MaximumElo = obj.MaximumElo
        };
    }

    public static Entities.Tournament ToEntity(TournamentDTO obj)
    {
        return new Entities.Tournament(
            id: obj.Id,
            name: obj.Name,
            description: obj.Description,
            startDate: obj.StartDate,
            format: obj.Format,
            gridType: obj.GridType,
            factionsBan: obj.FactionsBan,
            minimumElo: obj.MinimumElo,
            maximumElo: obj.MaximumElo)
        {
        };
    }

    public static List<TournamentDTO> FromEntitiesList(IEnumerable<Entities.Tournament> list)
    {
        return list.Select(FromEntity).ToList()!;
    }

    public static List<Entities.Tournament> ToEntitiesList(IEnumerable<TournamentDTO> list)
    {
        return list.Select(ToEntity).ToList()!;
    }
}
