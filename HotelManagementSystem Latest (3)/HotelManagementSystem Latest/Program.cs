using HotelManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// 1. Configure Database Connection
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 2. Add ASP.NET Core Identity with Roles
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        // Set RequireConfirmedAccount to false for local development so registered users can log in immediately
        options.SignIn.RequireConfirmedAccount = false;

        // Password requirements
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Lockout protection against brute-force attacks
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddRoles<IdentityRole>() // <-- Must be present for Role-Based Authorization
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Configure Cookie Security Policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true; // Prevents XSS attacks on cookies
    options.Cookie.SameSite = SameSiteMode.Strict; // Allows seamless redirects during login
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// 4. Set Page Authorization Conventions
builder.Services.AddRazorPages(options =>
{
    // Restrict all Bookings pages to authenticated users
    options.Conventions.AuthorizeFolder("/Bookings");

    // Restrict Room administrative actions strictly to Admins
    options.Conventions.AuthorizePage("/Rooms/Create", "AdminOnly");
    options.Conventions.AuthorizePage("/Rooms/Edit", "AdminOnly");
    options.Conventions.AuthorizePage("/Rooms/Delete", "AdminOnly");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

WebApplication app = builder.Build();

// 5. Database Migration & Role/Admin Seeding
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // 1. Ensure Roles Exist
    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    // 2. Ensure adminUser@hotel.com exists and has the Admin Role
    var adminEmail = "adminUser@hotel.com";
    var admin = await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        admin = new IdentityUser 
        { 
            UserName = adminEmail, 
            Email = adminEmail, 
            EmailConfirmed = true 
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
    else
    {
        // If user already exists, make sure they are in the Admin role
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
// 6. Request Pipeline Middleware
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();