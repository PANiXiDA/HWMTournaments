﻿using Common.Enums;

using Entities.Core;

namespace Entities;

public sealed class Settings : BaseEntity<int>
{
    public SettingType SettingType { get; set; }
    public string Value { get; set; }

    public Settings(
        int id,
        SettingType settingType,
        string value) : base(id)
    {
        Id = id;
        SettingType = settingType;
        Value = value;
    }
}
