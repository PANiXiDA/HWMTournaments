using Duende.IdentityModel;
using Duende.IdentityServer.Models;

using static Common.Constants.IdentityServiceConstants;

namespace IdentityService.IdentityProvider.Resources;

public static class IdentityResourcesConfig
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };
    }

    public static IEnumerable<ApiResource> GetApiResources()
    {
        return new List<ApiResource>
        {
            new ApiResource(DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName, DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName)
            {
                ApiSecrets = { new Secret(DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPISecret.Sha256()) },
                Scopes = new List<string>{ DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName },
                UserClaims = new List<string>{ JwtClaimTypes.Name, JwtClaimTypes.Role, JwtClaimTypes.Subject }
            },
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new List<ApiScope>
        {
            new ApiScope(
                name: DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName,
                displayName: DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName
            )
        };
    }
}
