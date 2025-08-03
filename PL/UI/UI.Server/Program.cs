using BL.DependencyInjection;

using Common.Constants.ServiceConfiguration;

using DAL.DependencyInjection;

using Dev.Template.AspNetCore.API.Extensions.Configurations;
using Dev.Template.AspNetCore.API.Middlewares;

using UI.Client.Services;
using UI.Server.Components;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

#region Custom Extensions

builder.Services.ConfigureGrpcClients();

builder.Services.UseDAL(builder.Configuration);
builder.Services.UseBL();

builder.Services.AddAuthenticationAndAuthorization(builder.Configuration);
builder.Services.AddSwagger(ServiceNamesConstants.ApiGateway);

#endregion

builder.Services.AddControllers();

builder.Services.AddScoped<NotificationService>();

builder.Services.AddHttpClient("ServerAPI", (sp, client) =>
{
    var baseAddress = builder.Configuration["HttpClients:ServerAPI:BaseAddress"];
    if (string.IsNullOrWhiteSpace(baseAddress))
    {
        throw new InvalidOperationException("BaseAddress для ServerAPI не настроен в appsettings.json");
    }
    client.BaseAddress = new Uri(baseAddress);
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
