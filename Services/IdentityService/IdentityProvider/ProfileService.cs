using System.Security.Claims;

using DAL.DbModels.Identity;

using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

using Microsoft.AspNetCore.Identity;

namespace IdentityService.IdentityProvider;

public class ProfileService : IProfileService
{
    protected UserManager<ApplicationUser> _userManager;

    public ProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var applicationUserId = context.Subject.FindFirst(JwtClaimTypes.Subject)?.Value;
        if (string.IsNullOrEmpty(applicationUserId))
        {
            return;
        }

        var applicationUser = await _userManager.FindByIdAsync(applicationUserId);
        if (applicationUser == null)
        {
            return;
        }

        var customClaims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Name, applicationUser.UserName ?? string.Empty)
        };

        var storedClaims = await _userManager.GetClaimsAsync(applicationUser);
        customClaims.AddRange(storedClaims);

        var roles = await _userManager.GetRolesAsync(applicationUser);
        customClaims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
        context.IssuedClaims.AddRange(customClaims);
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var applicationUser = await _userManager.GetUserAsync(context.Subject);
        context.IsActive = applicationUser != null;
    }
}
