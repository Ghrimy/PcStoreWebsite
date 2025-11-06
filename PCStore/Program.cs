using System.Net;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using PCStore.Components;
using PCStore.Components.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// Register Auth services
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<TokenAuthorizationHandler>();

builder.Services.AddHttpClient("ApiClient", client =>
    {
        client.BaseAddress = new Uri("https://localhost:5005/");
    })
    .ConfigureHttpClient(client =>
    {
        // The token will be added automatically via a delegating handler
    });

builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());

builder.Services.AddHttpClient("ApiWithAuth", client =>
{
    client.BaseAddress = new Uri("https://localhost:5005/");
}).AddHttpMessageHandler<TokenAuthorizationHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiWithAuth"));
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddAuthorizationCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Always use HTTPS
app.UseHttpsRedirection();

// Use static files (CSS, JS, etc.)
app.UseStaticFiles();

app.UseAntiforgery();
// Map your Blazor app
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseAuthentication();
// Optional: map other API or health check endpoints here if needed
//app.MapControllers();

app.Run();