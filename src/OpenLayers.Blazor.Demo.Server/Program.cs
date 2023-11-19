using OpenLayers.Blazor.Demo.Server.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

// workaround to make support different rendermodes
OpenLayers.Blazor.Demo.Components.RenderMode.DefaultRenderMode = new Microsoft.AspNetCore.Components.Web.InteractiveServerRenderMode();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(OpenLayers.Blazor.Demo.Components.Pages.Index).Assembly)
    .AddInteractiveServerRenderMode();

app.Run();
