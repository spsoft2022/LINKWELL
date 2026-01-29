using LinkwellProductionSystem.Data;
using LinkwellProductionSystem.Middlewares;
using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// ? Global exception handling
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Station}/{action=Index}/{id?}");
app.MapControllers();


app.Run();  