using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI.Client.Clients.DependencyInjection;
using UI.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.UseHttpClients(builder.Configuration);
builder.Services.UseServices();

await builder.Build().RunAsync();
