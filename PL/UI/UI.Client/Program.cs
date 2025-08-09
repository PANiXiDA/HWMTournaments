using Common.Constants;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration[AppsettingsKeysConstants.ServerAPIBaseAddress]  ?? builder.HostEnvironment.BaseAddress)
});

builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
