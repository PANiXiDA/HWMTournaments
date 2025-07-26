using Common.ConvertParams;

namespace DAL.Implementations.Includes;

internal static class SettingsIncludes
{
    internal static IQueryable<DbModels.Settings> Include(this IQueryable<DbModels.Settings> dbObjects, SettingsConvertParams convertParams)
    {
        return dbObjects;
    }
}
