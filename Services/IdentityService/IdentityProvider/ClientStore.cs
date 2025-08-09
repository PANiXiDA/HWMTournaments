using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;

using Microsoft.IdentityModel.Tokens;

using static Common.Constants.IdentityServiceConstants;

using GrantTypes = Common.Constants.IdentityServiceConstants.GrantTypes;

namespace IdentityService.IdentityProvider;

public class ClientStore : IClientStore
{
    private readonly ILogger<ClientStore> _logger;

    private readonly int _accessTokenLifeTimeInMinutes;
    private readonly int _refreshTokenLifeTimeInDays;
    private readonly string _clientSecret;

    private readonly List<Client> _clients;

    public ClientStore(
        ILogger<ClientStore> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        _accessTokenLifeTimeInMinutes = configuration.GetValue<int?>("IdentityServerSettings:AccessTokenLifeTimeInMinutes")
            ?? throw new InvalidOperationException("Не задана настройка IdentityServerSettings:AccessTokenLifeTimeInMinutes");
        _refreshTokenLifeTimeInDays = configuration.GetValue<int?>("IdentityServerSettings:RefreshTokenLifeTimeInDays")
            ?? throw new InvalidOperationException("Не задана настройка IdentityServerSettings:RefreshTokenLifeTimeInDays");
        _clientSecret = configuration["IdentityServerSettings:ClientSecret"]
            ?? throw new InvalidOperationException("Не задана настройка IdentityServerSettings:ClientSecret");

        _clients = BuildClients();
    }

    private List<Client> BuildClients()
    {
        return new List<Client>
        {
            CreateClient(Clients.Blazor, Clients.Blazor)
        };
    }

    private Client CreateClient(string clientId, string clientName)
    {
        return new Client
        {
            ClientId = clientId,
            ClientName = clientName,
            AccessTokenType = AccessTokenType.Jwt,
            AllowAccessTokensViaBrowser = false,
            AlwaysIncludeUserClaimsInIdToken = true,
            RequireConsent = false,
            AllowRememberConsent = true,
            AllowOfflineAccess = true,
            AlwaysSendClientClaims = true,
            AllowedIdentityTokenSigningAlgorithms = new List<string> { SecurityAlgorithms.RsaSha256 },

            AllowedGrantTypes = new List<string>
            {
                GrantType.ResourceOwnerPassword,
                GrantType.ClientCredentials,
                GrantTypes.Login
            },

            ClientSecrets =
            {
                new Secret(_clientSecret.Sha256())
            },

            Claims = new List<ClientClaim>
            {
                new ClientClaim(JwtClaimTypes.Role, JwtClaimTypes.Role),
                new ClientClaim(JwtClaimTypes.Name, JwtClaimTypes.Name)
            },

            AllowedScopes =
            {
                IdentityServerConstants.StandardScopes.OpenId,
                IdentityServerConstants.StandardScopes.Profile,
                DevTemplateAspNetCoreAPI.DevTemplateAspNetCoreAPIName,
                IdentityServerConstants.StandardScopes.OfflineAccess
            },

            RefreshTokenUsage = TokenUsage.OneTimeOnly,
            RefreshTokenExpiration = TokenExpiration.Absolute,
            AccessTokenLifetime = (int)TimeSpan.FromMinutes(_accessTokenLifeTimeInMinutes).TotalSeconds,
            AbsoluteRefreshTokenLifetime = (int)TimeSpan.FromDays(_refreshTokenLifeTimeInDays).TotalSeconds,
            UpdateAccessTokenClaimsOnRefresh = true
        };
    }

    public Task<Client?> FindClientByIdAsync(string clientId)
    {
        var client = _clients.FirstOrDefault(item => item.ClientId == clientId);
        return Task.FromResult(client);
    }
}
