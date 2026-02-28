using BilkoNavigator_.Data;
using BilkoNavigator_.Data.Seed;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>();
var app = builder.Build();

//Seed data 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var env = services.GetRequiredService<IWebHostEnvironment>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    const string adminRole = "Admin";
    const string userRole = "User";

    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    if (!await roleManager.RoleExistsAsync(userRole))
    {
        await roleManager.CreateAsync(new IdentityRole(userRole));
    }

    var seedAdminEmail = app.Configuration["Admin:SeedEmail"];
    var seedAdminPassword = app.Configuration["Admin:SeedPassword"];

    if (string.IsNullOrWhiteSpace(seedAdminEmail))
        seedAdminEmail = "dudito920@gmail.com";

    if (string.IsNullOrWhiteSpace(seedAdminPassword))
        seedAdminPassword = "Noit2026!";

    var seededAdmin = await userManager.FindByEmailAsync(seedAdminEmail);
    if (seededAdmin == null)
    {
        seededAdmin = new User
        {
            UserName = seedAdminEmail,
            Email = seedAdminEmail,
            EmailConfirmed = true,
            UserStatus = "Active"
        };

        var createAdminResult = await userManager.CreateAsync(seededAdmin, seedAdminPassword);
        if (!createAdminResult.Succeeded)
        {
            var reason = string.Join("; ", createAdminResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not seed admin user: {reason}");
        }
    }

    if (!await userManager.IsInRoleAsync(seededAdmin, adminRole))
    {
        await userManager.AddToRoleAsync(seededAdmin, adminRole);
    }

    var configuredAdminEmail = app.Configuration["Admin:Email"];
    if (!string.IsNullOrWhiteSpace(configuredAdminEmail))
    {
        var adminUser = await userManager.FindByEmailAsync(configuredAdminEmail);
        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, adminRole))
        {
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }

    foreach (var user in userManager.Users.ToList())
    {
        if (await userManager.IsInRoleAsync(user, adminRole))
            continue;

        if (!await userManager.IsInRoleAsync(user, userRole))
        {
            await userManager.AddToRoleAsync(user, userRole);
        }
    }

    await HerbSeeder.SeedAsync(context, env);
}


// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

// Explicitly map Razor Pages
app.MapRazorPages();

// Explicitly map controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure HerbsController is mapped
app.MapControllerRoute(
    name: "herbs",
    pattern: "Herbs/{action=Index}/{id?}");

//следващото е излишно
app.MapRazorPages();

app.Run();
