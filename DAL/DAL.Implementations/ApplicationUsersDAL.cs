using DAL.DbModels.Identity;

using Google.Protobuf.WellKnownTypes;

using ApplicationUserRole = Gen.IdentityService.Enums.ApplicationUserRole;
using Enum = System.Enum;

namespace DAL.Implementations;

internal sealed class ApplicationUsersDAL
{
    internal static Gen.IdentityService.Entities.ApplicationUser ConvertDbObjectToProto(ApplicationUser dbObject)
    {
        var proto = new Gen.IdentityService.Entities.ApplicationUser()
        {
            Id = dbObject.Id,
            Name = dbObject.UserName,
            Password = string.Empty,
            Email = dbObject.Email,
            EmailConfirmed = dbObject.EmailConfirmed,
            PhoneNumber = dbObject.PhoneNumber,
            NormalizedName = dbObject.NormalizedUserName,
            NormalizedEmail = dbObject.NormalizedEmail,
            SecurityStamp = dbObject.SecurityStamp,
            ConcurrencyStamp = dbObject.ConcurrencyStamp,
            PhoneNumberConfirmed = dbObject.PhoneNumberConfirmed,
            TwoFactorEnabled = dbObject.TwoFactorEnabled,
            LockoutEnd = dbObject.LockoutEnd.HasValue ? Timestamp.FromDateTimeOffset(dbObject.LockoutEnd.Value) : null,
            LockoutEnabled = dbObject.LockoutEnabled,
            AccessFailedCount = dbObject.AccessFailedCount
        };

        var roles = dbObject.Roles
            .Select(item => (ApplicationUserRole)Enum.Parse(
                typeof(ApplicationUserRole),
                item.Role?.Name ?? string.Empty,
                ignoreCase: true))
            .ToList();
        proto.Roles.AddRange(roles);

        return proto;
    }
}
