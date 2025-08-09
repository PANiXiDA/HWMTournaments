using Common.ConvertParams;
using Common.SearchParams;

using DAL.EF;
using DAL.Implementations.Core;
using DAL.Implementations.Filters;
using DAL.Implementations.Includes;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public sealed class TournamentsDAL : BaseDAL<DefaultDbContext, DbModels.Tournament, Entities.Tournament, int, TournamentsSearchParams, TournamentsConvertParams>, ITournamentsDAL
{
    protected override bool RequiresUpdatesAfterObjectSaving => false;

    public TournamentsDAL(DefaultDbContext context) : base(context)
    {
    }

    protected override Task UpdateBeforeSavingAsync(Entities.Tournament entity, DbModels.Tournament dbObject)
    {
        dbObject.Name = entity.Name;
        dbObject.Description = entity.Description;
        dbObject.StartDate = entity.StartDate;
        dbObject.Format = entity.Format;
        dbObject.GridType = entity.GridType;
        dbObject.FactionsBan = entity.FactionsBan;
        dbObject.MinimumElo = entity.MinimumElo;
        dbObject.MaximumElo = entity.MaximumElo;

        return Task.CompletedTask;
    }

    protected override async Task<IQueryable<DbModels.Tournament>> BuildDbQueryAsync(IQueryable<DbModels.Tournament> dbObjects, TournamentsSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Entities.Tournament>> BuildEntitiesListAsync(IQueryable<DbModels.Tournament> dbObjects, TournamentsConvertParams convertParams)
    {
        return (await dbObjects.Include(convertParams).ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
    }

    internal static Entities.Tournament ConvertDbObjectToEntity(DbModels.Tournament dbObject)
    {
        return new Entities.Tournament(
            id: dbObject.Id,
            name: dbObject.Name,
            description: dbObject.Description,
            startDate: dbObject.StartDate,
            format: dbObject.Format,
            gridType: dbObject.GridType,
            factionsBan: dbObject.FactionsBan,
            minimumElo: dbObject.MinimumElo,
            maximumElo: dbObject.MaximumElo)
        {
        };
    }
}
