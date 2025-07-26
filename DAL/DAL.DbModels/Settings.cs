using Common.Enums;

using DAL.DbModels.Core;

namespace DAL.DbModels;

public sealed class Settings : BaseDbModel<int>
{
    public SettingType SettingType { get; set; }
    public string Value { get; set; } = string.Empty;
}
