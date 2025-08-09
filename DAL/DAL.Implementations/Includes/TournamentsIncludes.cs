using Common.ConvertParams;

namespace DAL.Implementations.Includes;

internal static class TournamentsIncludes
{
    internal static IQueryable<DbModels.Tournament> Include(this IQueryable<DbModels.Tournament> dbObjects, TournamentsConvertParams convertParams)
    {
        return dbObjects;
    }
}
