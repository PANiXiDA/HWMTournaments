using System.Security.Claims;

namespace Dev.Template.AspNetCore.API.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetApplicationUserId(this ClaimsPrincipal applicationUser) => int.Parse(applicationUser.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}
