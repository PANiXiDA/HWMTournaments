using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using DAL.EF;
using DAL.Implementations.Core;
using DAL.Implementations.Filters;
using DAL.Implementations.Includes;
using DAL.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations;

public sealed class SettingsDAL : BaseDAL<DefaultDbContext, DbModels.Settings, Entities.Settings, int, SettingsSearchParams, SettingsConvertParams>, ISettingsDAL
{
    protected override bool RequiresUpdatesAfterObjectSaving => false;

    public SettingsDAL(DefaultDbContext context) : base(context)
    {
    }

    protected override Task UpdateBeforeSavingAsync(Entities.Settings entity, DbModels.Settings dbObject)
    {
        dbObject.SettingType = entity.SettingType;
        dbObject.Value = entity.Value;

        return Task.CompletedTask;
    }

    protected override async Task<IQueryable<DbModels.Settings>> BuildDbQueryAsync(IQueryable<DbModels.Settings> dbObjects, SettingsSearchParams searchParams)
    {
        dbObjects = await base.BuildDbQueryAsync(dbObjects, searchParams);
        return dbObjects.Filter(searchParams);
    }

    protected override async Task<IList<Entities.Settings>> BuildEntitiesListAsync(IQueryable<DbModels.Settings> dbObjects, SettingsConvertParams convertParams)
    {
        return (await dbObjects.Include(convertParams).ToListAsync()).Select(ConvertDbObjectToEntity).ToList();
    }

    internal static Entities.Settings ConvertDbObjectToEntity(DbModels.Settings dbObject)
    {
        return new Entities.Settings(
            id: dbObject.Id,
            settingType: dbObject.SettingType,
            value: dbObject.Value);
    }

    public async Task<Entities.Settings?> GetAsync(SettingType settingType)
    {
        return (await GetAsync(item => item.SettingType == settingType)).FirstOrDefault();
    }
}
