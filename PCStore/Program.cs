using System.Net;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using PCStore.Components;
using PCStore.Components.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMudServices();
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthService>(sp =>
{
    var client = sp.GetRequiredService<IHttpClientFactory>().CreateClient("API");
    return new AuthService(client);
});



builder.Services.AddScoped<CustomAuthStateProvider>();

// Add services to the container. 
//Cookies
var cookieContainer = new CookieContainer();
builder.Services.AddSingleton(new CookieContainer());

builder.Services.AddHttpClient("API", client =>
    {
        client.BaseAddress = new Uri("https://localhost:5005/");
    })
    .ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
    {
        UseCookies = true,
        CookieContainer = sp.GetRequiredService<CookieContainer>(),
    });

builder.Services.AddAuthorizationCore();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();