using Common.Constants;

using Gen.IdentityService.ApplicationUserService;

namespace UI.Server.Extensions.Configurations;

public static class GrpcClientConfiguration
{
    public static void ConfigureGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<ApplicationUserService.ApplicationUserServiceClient>(options => options.Address = new Uri(configuration.GetValue<string>(AppsettingsKeysConstants.IdentityServiceBaseAddress)!));
    }
}
