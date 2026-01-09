using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDefaultIdentity<User>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = true;
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 6;
//    options.Password.RequiredUniqueChars = 0;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireUppercase = false;
//})
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>();

//Add Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddRazorPages();



builder.Services.AddControllersWithViews();


//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 6;
//    options.Password.RequiredUniqueChars = 0;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireLowercase = false;
//    options.Password.RequireUppercase = false;
//});

builder.Services.AddRazorPages();

// Register IEmailSender with a basic implementation
builder.Services.AddTransient<IEmailSender, EmailSender>();

//builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, EmailSender>();

var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); // Ensure authentication middleware is added
app.UseAuthorization();

app.MapRazorPages(); // Map Razor Pages
app.MapDefaultControllerRoute();

app.Run();
