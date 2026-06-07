using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Data;
using TechMoveGLMS.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== FORCE INVARIANT CULTURE TO AVOID DECIMAL PARSING ISSUES =====
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
// ===================================================================

// Add services to the container
builder.Services.AddControllersWithViews();

// Add SQL Server Database Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register custom services
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IFileService, FileService>();

// Register HttpClient for Currency API (ExchangeRate-API)
builder.Services.AddHttpClient<ICurrencyService, CurrencyService>(client =>
{
    client.BaseAddress = new Uri("https://v6.exchangerate-api.com/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(15);
});

// ===== API SERVICE FOR MVC TO CALL BACKEND API =====
builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5001";
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
});
// ====================================================

// ===== SESSION SUPPORT =====
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// ===========================

// ===== COOKIE AUTHENTICATION - THIS IS REQUIRED! =====
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });
// =====================================================

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ===== MIDDLEWARE ORDER - ORDER MATTERS! =====
app.UseSession();           // Session first
app.UseAuthentication();    // Authentication second (MUST be before Authorization)
app.UseAuthorization();     // Authorization third
// ============================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Auto-create database on startup (development only)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.EnsureCreated();
        Console.WriteLine("SQL Server database created/verified successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database error: {ex.Message}");
        Console.WriteLine("Make sure SQL Server is running and connection string is correct.");
    }
}

app.Run();