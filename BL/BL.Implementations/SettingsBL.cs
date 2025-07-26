using BL.Interfaces;

using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;
using Common.SearchParams.Core;

using DAL.Interfaces;

using Entities;

namespace BL.Implementations;

public sealed class SettingsBL : ISettingsBL
{
    private readonly ISettingsDAL _settingsDAL;

    public SettingsBL(ISettingsDAL settingsDAL)
    {
        _settingsDAL = settingsDAL;
    }

    public Task<Settings> GetAsync(int id, SettingsConvertParams? convertParams = null)
    {
        return _settingsDAL.GetAsync(id, convertParams);
    }

    public Task<SearchResult<Settings>> GetAsync(SettingsSearchParams searchParams, SettingsConvertParams? convertParams = null)
    {
        return _settingsDAL.GetAsync(searchParams, convertParams);
    }

    public Task<Settings?> GetAsync(SettingType settingType)
    {
        return _settingsDAL.GetAsync(settingType);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _settingsDAL.ExistsAsync(id);
    }

    public Task<bool> ExistsAsync(SettingsSearchParams searchParams)
    {
        return _settingsDAL.ExistsAsync(searchParams);
    }

    public async Task<int> AddOrUpdateAsync(Settings entity)
    {
        entity.Id = await _settingsDAL.AddOrUpdateAsync(entity);
        return entity.Id;
    }

    public async Task<IList<int>> AddOrUpdateAsync(IList<Settings> entities)
    {
        return await _settingsDAL.AddOrUpdateAsync(entities);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _settingsDAL.DeleteAsync(id);
    }

    public Task<bool> DeleteAsync(List<int> ids)
    {
        return _settingsDAL.DeleteAsync(db => ids.Contains(db.Id));
    }
}
