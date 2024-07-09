using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Stripe settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Configure Identity (Commented out for now)
// This section sets up Identity with default token providers and specifies the paths for login, logout, and access denied.
// Uncomment these lines if you need Identity services.
// builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();
//
// builder.Services.ConfigureApplicationCookie(options => {
//     options.LoginPath = $"/Identity/Account/Login";
//     options.LogoutPath = $"/Identity/Account/Logout";
//     options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
// });
// 
// builder.Services.AddAuthentication().AddFacebook(option => {
//     option.AppId = "193813826680436";
//     option.AppSecret = "8fc42ae3f4f2a4986143461d4e2da919";
// });
// builder.Services.AddAuthentication().AddMicrosoftAccount(option => {
//     option.ClientId = "ec4d380d-d631-465d-b473-1e26ee706331";
//     option.ClientSecret = "qMW8Q~LlEEZST~SDxDgcEVx_45LJQF2cQ_rEKcSQ";
// });

// Configure session state
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register application services
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();  // Needed to support Razor Pages
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();  // Service for sending emails

// Configure Swagger for API documentation and testing
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BulkyBook API", Version = "v1" });
});

var app = builder.Build();  // Build the application

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Enable HSTS (HTTP Strict Transport Security) for production
}

app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS
app.UseStaticFiles();  // Serve static files from wwwroot

// Configure Stripe API key
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

// Add middleware for authentication and authorization (Commented out for now)
// Uncomment these lines if you need authentication and authorization middleware.
// app.UseAuthentication();  // Check if user credentials are valid
// app.UseAuthorization();  // Check if user is authorized to access the resource

app.UseSession();  // Enable session state

// Seed the database with initial data
void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
SeedDatabase();

// Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BulkyBook API v1");
    c.RoutePrefix = string.Empty;  // Set Swagger UI at the app's root
});

// Map Razor Pages
app.MapRazorPages();

// Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();  // Run the application
