using Common.SearchParams;

namespace DAL.Implementations.Filters;

internal static class UsersFilters
{
    internal static IQueryable<DbModels.User> Filter(this IQueryable<DbModels.User> dbObjects, UsersSearchParams searchParams)
    {
        return dbObjects;
    }
}
