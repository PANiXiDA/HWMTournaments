using System.Security.Claims;

namespace UI.Server.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetApplicationUserId(this ClaimsPrincipal applicationUser) => int.Parse(applicationUser.FindFirst(ClaimTypes.NameIdentifier)!.Value);
}
