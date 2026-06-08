// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.HttpServices;
using GLMS.AppServices;
using GLMS.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Session for storing JWT token received from the API
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cookie authentication for MVC pages
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// IHttpContextAccessor so TokenHandler can read the JWT from session
builder.Services.AddHttpContextAccessor();

// TokenHandler attaches the JWT token to all outgoing API requests
builder.Services.AddTransient<TokenHandler>();

// The base URL of the GLMS Web API
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]!;

// Named HttpClient for AuthController (no token needed for login)
builder.Services.AddHttpClient("GlmsApi", c => c.BaseAddress = new Uri(apiBaseUrl));

// Typed HttpClients for service layer — all go through TokenHandler
builder.Services.AddHttpClient<IClientAppService, HttpClientAppService>(
        c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<IContractAppService, HttpContractAppService>(
        c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<TokenHandler>();

builder.Services.AddHttpClient<IServiceRequestAppService, HttpServiceRequestAppService>(
        c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<TokenHandler>();

// CurrencyService and FileService stay in the MVC (no DB access needed)
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
