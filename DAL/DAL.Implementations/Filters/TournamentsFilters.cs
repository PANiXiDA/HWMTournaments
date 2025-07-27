using Common.SearchParams;

namespace DAL.Implementations.Filters;

internal static class TournamentsFilters
{
    internal static IQueryable<DbModels.Tournament> Filter(this IQueryable<DbModels.Tournament> dbObjects, TournamentsSearchParams searchParams)
    {
        if (!string.IsNullOrEmpty(searchParams.SearchQuery))
        {
            dbObjects = dbObjects.Where(item => item.Name.ToLower().Contains(searchParams.SearchQuery.ToLower()));
        }
        if (searchParams.Format.HasValue)
        {
            dbObjects = dbObjects.Where(item => item.Format == searchParams.Format.Value);
        }

        return dbObjects;
    }
}