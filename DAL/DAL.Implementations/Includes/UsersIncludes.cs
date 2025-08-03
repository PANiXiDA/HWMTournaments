using Common.ConvertParams;

using Microsoft.EntityFrameworkCore;

namespace DAL.Implementations.Includes;

internal static class UsersIncludes
{
    internal static IQueryable<DbModels.User> Include(this IQueryable<DbModels.User> dbObjects, UsersConvertParams convertParams)
    {
        if (convertParams.IncludeApplicationUser)
        {
            dbObjects = dbObjects.Include(item => item.ApplicationUser).ThenInclude(item => item!.Roles);
        }

        return dbObjects;
    }
}
