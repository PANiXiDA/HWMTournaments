using Common.ConvertParams;
using Common.SearchParams;

using DAL.EF;
using DAL.Implementations.Core;
using DAL.Implementations.Filters;
using DAL.Implementations.Includes;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public sealed class UsersDAL : BaseDAL<DefaultDbContext, DbModels.User, Entities.User, int, UsersSearchParams, UsersConvertParams>, IUsersDAL
{
    protected override bool RequiresUpdatesAfterObjectSaving => false;

    public UsersDAL(DefaultDbContext context) : base(context)
    {
    }

    protected override Task UpdateBeforeSavingAsync(Entities.User entity, DbModels.User dbObject)
    {
        return Task.CompletedTask;
    }

    protected override async Task<IQueryable<DbModels.User>> BuildDbQueryAsync(IQueryable<DbModels.User> dbObjects, UsersSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Entities.User>> BuildEntitiesListAsync(IQueryable<DbModels.User> dbObjects, UsersConvertParams convertParams)
    {
        return (await dbObjects.Include(convertParams).ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
    }

    internal static Entities.User ConvertDbObjectToEntity(DbModels.User dbObject)
    {
        return new Entities.User(
            id: dbObject.Id);
    }
}
