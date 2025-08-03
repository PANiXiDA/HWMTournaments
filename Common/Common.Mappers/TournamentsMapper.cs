namespace Common.Mappers;

using System.Collections.Generic;
using System.Linq;

using DTOs.Models;

using Entities;

public static class TournamentsMapper
{
    public static TournamentDTO EntityToDTO(this Tournament tournament)
    {
        return new TournamentDTO
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Description = tournament.Description,
            StartDate = tournament.StartDate,
            Format = tournament.Format,
            GridType = tournament.GridType,
            FactionsBan = tournament.FactionsBan,
            MinimumElo = tournament.MinimumElo,
            MaximumElo = tournament.MaximumElo
        };
    }

    public static Tournament DTOToEntity(this TournamentDTO dto)
    {
        return new Tournament(
            id: dto.Id,
            name: dto.Name,
            description: dto.Description,
            startDate: dto.StartDate,
            format: dto.Format,
            gridType: dto.GridType,
            factionsBan: dto.FactionsBan,
            minimumElo: dto.MinimumElo,
            maximumElo: dto.MaximumElo);
    }

    public static IEnumerable<TournamentDTO> FromEntityToDTOList(this IEnumerable<Tournament> list) => list.Select(EntityToDTO).ToList();

    public static IEnumerable<Tournament> FromDTOToEntityList(this IEnumerable<TournamentDTO> list) => list.Select(DTOToEntity).ToList();
}
