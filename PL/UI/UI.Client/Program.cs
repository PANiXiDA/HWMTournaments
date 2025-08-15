using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using UI.Client.Clients.DependencyInjection;
using UI.Client.Repositories.DependencyInjection;
using UI.Client.Services.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.UseHttpClients(builder.Configuration);
builder.Services.UseServices();
builder.Services.UseRepositories();

await builder.Build().RunAsync();
