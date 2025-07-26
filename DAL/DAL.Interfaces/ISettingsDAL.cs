using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using DAL.Interfaces.Core;

namespace DAL.Interfaces;

public interface ISettingsDAL : IBaseDAL<DbModels.Settings, Entities.Settings, int, SettingsSearchParams, SettingsConvertParams>
{
    Task<Entities.Settings?> GetAsync(SettingType settingType);
}
