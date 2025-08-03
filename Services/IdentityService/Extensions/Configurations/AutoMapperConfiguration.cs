using IdentityService.Mappers;

namespace IdentityService.Extensions.Configurations;

public static class AutoMapperConfiguration
{
    public static void UseAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(ApplicationUserMapper).Assembly);
    }
}
