using Common.Constants;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.Configuration.GetValue<string>(AppsettingsKeysConstants.ServerAPIBaseAddress)  ?? builder.HostEnvironment.BaseAddress)
});
builder.Services.AddHttpClient(ClientsConstants.IdentityService, client =>
{
    var baseAddr = builder.Configuration.GetValue<string>(AppsettingsKeysConstants.IdentityServiceHTTPBaseAddress) ?? builder.HostEnvironment.BaseAddress;
    client.BaseAddress = new Uri(baseAddr);
});

builder.Services.UseServices();

await builder.Build().RunAsync();
