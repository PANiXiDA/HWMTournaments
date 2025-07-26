using Common.ConvertParams;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations.Includes;

internal static class UsersIncludes
{
    internal static IQueryable<DbModels.User> Include(this IQueryable<DbModels.User> dbObjects, UsersConvertParams convertParams)
    {
        return dbObjects;
    }
}
