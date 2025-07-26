using BL.Interfaces.Core;

using Common.ConvertParams;
using Common.Enums;
using Common.SearchParams;

using Entities;

namespace BL.Interfaces;

public interface ISettingsBL : ICrudBL<Settings, int, SettingsSearchParams, SettingsConvertParams>
{
    Task<Settings?> GetAsync(SettingType settingType);
}

