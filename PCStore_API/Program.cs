using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PCStore_API.ApiResponse;
using PCStore_API.Data;
using PCStore_API.Services.OrderServices;
using PCStore_API.Services.ProductServices;
using PCStore_API.Services.ShoppingCartServices;
using PCStore_API.Services.UserService;
using PCStore_API.Services.UserService.Security;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configures application db context and services
builder.Services.AddDbContext<PcStoreDbContext>();
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

//Database connection
builder.Services
    .AddDbContext<PcStoreDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:5002")
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = "AuthCookie";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None; // critical when frontend is on another port
        options.Cookie.Path = "/";
        options.Cookie.Domain = null; // stay local
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });


builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Add exception handling middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowFrontend");
app.Use((context, next) =>
{
    context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    return next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();