using Common.Configuration.Mail.Implementations;
using Common.Configuration.Mail.Interfaces;
using Common.Configuration.Mail.Models;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Configuration.Mail.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection UseEmailSender(this IServiceCollection services, IConfiguration configuration, string sectionName = "SmtpConfiguration")
    {
        services.AddOptions<SmtpConfiguration>().Bind(configuration.GetSection(sectionName));
        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
