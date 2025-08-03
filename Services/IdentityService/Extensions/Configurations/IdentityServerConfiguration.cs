using System.Security.Cryptography.X509Certificates;

using DAL.DbModels.Identity;
using DAL.EF;

using IdentityService.IdentityProvider;
using IdentityService.IdentityProvider.Resources;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

using static Common.Constants.IdentityServiceConstants;

namespace IdentityService.Extensions.Configurations;

public static class IdentityServerConfiguration
{
    public static void ConfigureIdentityServer(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureIdentityOptions(services, configuration);
        ConfigureCorsPolicy(services);
        ConfigureDataProtectionAndPersistedGrantStore(services);
        ConfigureAspNetIdentity(services, configuration);
        ConfigureIdentityServerServices(services, configuration);
    }

    private static void ConfigureIdentityOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
    }

    private static void ConfigureCorsPolicy(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.AllowAll, builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }

    private static void ConfigureDataProtectionAndPersistedGrantStore(IServiceCollection services)
    {
        services.AddDataProtection().PersistKeysToDbContext<DefaultDbContext>();
    }

    private static void ConfigureAspNetIdentity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
        {
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultPhoneProvider;
        })
            .AddEntityFrameworkStores<DefaultDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void ConfigureIdentityServerServices(IServiceCollection services, IConfiguration configuration)
    {
        var identityServerSettingsSection = configuration.GetSection("IdentityServerSettings");

        var certificateName = identityServerSettingsSection["CertificateName"]
            ?? throw new InvalidOperationException("Не задана настройка IdentityServerSettings:CertificateName");
        var certificatePass = identityServerSettingsSection["CertificatePassword"]
            ?? throw new InvalidOperationException("Не задана настройка IdentityServerSettings:CertificatePassword");
        var certificate = new X509Certificate2(certificateName, certificatePass);
        var key = new X509SecurityKey(certificate);

        var eventsSection = identityServerSettingsSection.GetSection("Events");

        services.AddIdentityServer(options =>
        {
            options.Events.RaiseErrorEvents = eventsSection.GetValue<bool>("RaiseErrorEvents");
            options.Events.RaiseFailureEvents = eventsSection.GetValue<bool>("RaiseFailureEvents");
            options.Events.RaiseInformationEvents = eventsSection.GetValue<bool>("RaiseInformationEvents");
            options.Events.RaiseSuccessEvents = eventsSection.GetValue<bool>("RaiseSuccessEvents");
        })
            .AddAspNetIdentity<ApplicationUser>()
            .AddInMemoryApiScopes(IdentityResourcesConfig.GetApiScopes())
            .AddInMemoryIdentityResources(IdentityResourcesConfig.GetIdentityResources())
            .AddInMemoryApiResources(IdentityResourcesConfig.GetApiResources())
            .AddSigningCredential(new SigningCredentials(key, SecurityAlgorithms.RsaSha256))
            .AddPersistedGrantStore<PersistedGrantStore>()
            .AddExtensionGrantValidator<ExtensionGrantValidator>()
            .AddProfileService<ProfileService>()
            .AddClientStore<ClientStore>();
    }
}
