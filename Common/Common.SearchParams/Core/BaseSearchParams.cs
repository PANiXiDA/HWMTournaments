﻿using System.ComponentModel.DataAnnotations;

using Common.Enums;

namespace Common.SearchParams.Core;

public class BaseSearchParams
{
    public int Page { get; set; }
    public int? ObjectsCount { get; set; }
    public string? SortField { get; set; }
    public SortOrder SortOrder { get; set; }

    [Display(Name = "Поиск по названию")]
    public string? SearchQuery { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateTime? UpdatedFrom { get; set; }
    public DateTime? UpdatedTo { get; set; }
    public DateTime? DeletedFrom { get; set; }
    public DateTime? DeletedTo { get; set; }

    public BaseSearchParams()
    {
        Page = 1;
    }

    public BaseSearchParams(int page = 1, int? objectsCount = null)
    {
        Page = page;
        ObjectsCount = objectsCount;
    }
}
