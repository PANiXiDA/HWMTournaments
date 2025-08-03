using Gen.IdentityService.Enums;

using Microsoft.AspNetCore.Identity;

namespace IdentityService.Extensions;

public static class RoleSynchronizerExtensions
{
    public static async Task SynchronizerEnumRolesAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

        var enumNames = Enum.GetNames<ApplicationUserRole>();

        foreach (var enumName in enumNames)
        {
            if (!await roleManager.RoleExistsAsync(enumName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole<int>(enumName));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Не удалось создать роль '{enumName}': " + string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
