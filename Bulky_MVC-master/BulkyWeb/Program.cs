using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;  // Import logging
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Step 1: Add services to the container.
builder.Services.AddControllersWithViews();

// Step 2: Configure the database context with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Step 3: Configure Stripe settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Step 4: Configure session state
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Step 5: Register application services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();  // Service for sending emails

// Step 6: Configure Swagger for API documentation and testing
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BulkyBook API", Version = "v1" });
});

// Step 7: Configure CORS to allow requests from port 3000
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();  // Build the application

// Step 8: Get a logger instance (Injection)
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Step 9: Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Enable HSTS (HTTP Strict Transport Security) for production
}

app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS
app.UseStaticFiles();  // Serve static files from wwwroot

// Step 10: Configure Stripe API key
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();

app.UseSession();  // Enable session state

// Step 11: Enable CORS middleware
app.UseCors("AllowReactApp");

// Step 12: Enable Swagger middleware
app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BulkyBook API v1");
    c.RoutePrefix = string.Empty;  // Set Swagger UI at the app's root
});

// Step 13: Global error handling middleware
app.Use(async (context, next) =>
{
    try
    {
        await next.Invoke();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An unhandled exception occurred.");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("An unexpected error occurred.");
    }
});

// Step 14: Map default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();  // Run the application


// ------------------------------------------------------------------
// Commented out code for optional features
// ------------------------------------------------------------------

// Configure Identity (Optional)
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

// Add Facebook Authentication (Optional)
// builder.Services.AddAuthentication().AddFacebook(option => {
//     option.AppId = "193813826680436";
//     option.AppSecret = "8fc42ae3f4f2a4986143461d4e2da919";
// });

// Add Microsoft Account Authentication (Optional)
// builder.Services.AddAuthentication().AddMicrosoftAccount(option => {
//     option.ClientId = "ec4d380d-d631-465d-b473-1e26ee706331";
//     option.ClientSecret = "qMW8Q~LlEEZST~SDxDgcEVx_45LJQF2cQ_rEKcSQ";
// });

// Enable Authentication and Authorization Middleware (Optional)
// Uncomment these lines if you need authentication and authorization middleware.
// app.UseAuthentication();  // Check if user credentials are valid
// app.UseAuthorization();  // Check if user is authorized to access the resource

// Map Razor Pages (Optional)
// Uncomment this line if you need to use Razor Pages.
// app.MapRazorPages();

// Seed the database with initial data (Optional)
// Uncomment this block if you need to initialize the database with initial data.
// void SeedDatabase()
// {
//     using (var scope = app.Services.CreateScope())
//     {
//         var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
//         dbInitializer.Initialize();
//     }
// }
// SeedDatabase();
