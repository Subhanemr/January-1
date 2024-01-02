using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiShop.DAL;
using MultiShop.Interfaces;
using MultiShop.Middlewares;
using MultiShop.Models;
using MultiShop.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt => { opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AppDbContextInitializer>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;

    options.User.RequireUniqueEmail = true;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);

    options.SignIn.RequireConfirmedEmail = true;
}

).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();
    initializer.InitializeDbContext().Wait();
    initializer.CreateUserRoles().Wait();
    initializer.InitializeAdmin().Wait();
}


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllerRoute("default",
    "{area:exists}/{controller=home}/{action=index}/{id?}");

app.MapControllerRoute("default",
    "{controller=home}/{action=index}/{id?}");

app.Run();
