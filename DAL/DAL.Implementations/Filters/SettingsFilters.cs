using Common.SearchParams;

namespace DAL.Implementations.Filters;

internal static class SettingsFilters
{
    internal static IQueryable<DbModels.Settings> Filter(this IQueryable<DbModels.Settings> dbObjects, SettingsSearchParams searchParams)
    {
        return dbObjects;
    }
}
