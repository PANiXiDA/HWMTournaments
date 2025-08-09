using BL.DependencyInjection;

using Common.Constants;
using Common.Constants.ServiceConfiguration;

using DAL.DependencyInjection;

using Microsoft.AspNetCore.Components;

using UI.Client.Services;
using UI.Server.Components;
using UI.Server.Extensions.Configurations;
using UI.Server.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region Custom Extensions

builder.Services.ConfigureGrpcClients(builder.Configuration);

builder.Services.UseDAL(builder.Configuration);
builder.Services.UseBL();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSwagger(ServiceNamesConstants.ApiGateway);

#endregion

builder.Services.AddControllers();

builder.Services.AddScoped<NotificationService>();

builder.Services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    return new HttpClient { BaseAddress = new Uri(nav.BaseUri) };
});

var app = builder.Build();

#region Middlewares

app.UseMiddleware<ExceptionsMiddleware>();
app.UseMiddleware<RequestMiddleware>();
app.UseMiddleware<LoggingMiddleWare>();

#endregion

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

#region Custom Extensions

app.UseSwaggerAndSwaggerUI();
app.UseAuthenticationAndAuthorization();

#endregion

app.MapControllers();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(UI.Client._Imports).Assembly);

app.Run();
