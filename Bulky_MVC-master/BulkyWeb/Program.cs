using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BulkyBook.Utility;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Part 1: Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options=>                               // When creating identity, there is a chance that another is created instead of existing one, whcih could lead to errors. Careful.
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// builder.Services.AddDefaultIdentity<IdentityUser> gets added by default when scaffolding Identity. It tells email account should be confirmed if you signed in. Not required
// AddEntityFrameworkStores<ApplicationDbContext>() this is to tell .NET that all the database tables needed for the identity will be managed with the help of dbcontext, which isApplicationDbContext.
// We are binding EF with Identity Tables with .AddEntityFrameworkStores<ApplicationDbContext>()
// Use ApplicationUser if you want to add more than default IdentityUser by declaring it in the model and passing in on Register razor page
// Add IdentityRole to assign roles to users. But AddDefaultIdentity<IdentityUser> does not allow it. So, we will have to use AddIdentity<IdentityUser,IdentityRole>()
builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();     //  AddDefaultTokenProviders is needed for Register page's GenerateEmailConfirmationTokenAsync
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});
builder.Services.AddAuthentication().AddFacebook(option => {
    option.AppId = "193813826680436";
    option.AppSecret = "8fc42ae3f4f2a4986143461d4e2da919";
});
builder.Services.AddAuthentication().AddMicrosoftAccount(option => {
    option.ClientId = "ec4d380d-d631-465d-b473-1e26ee706331";
    option.ClientSecret = "qMW8Q~LlEEZST~SDxDgcEVx_45LJQF2cQ_rEKcSQ";
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();                               // Needed to have razor working
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();        // Subscribing email sender so our program knows how to handle it
var app = builder.Build();      // Part 1: Once all the dependency injections are added, build the app to run

// Part 1: Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // Part 1: To configure wwwroot files in our application

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseRouting();
app.UseAuthentication();        // Add middleware authentication before authorization: checks if username and password is valid
app.UseAuthorization();         // Authorization comes into picture if authentication is valid. This defines roles based on which, pages are available.

app.UseSession();
SeedDatabase();
app.MapRazorPages();            // Pipeline here needs to have map razor pages for the routing. Now add migrations, which will automatically create the necessary tables. Then update 
app.MapControllerRoute( // Part 1: Default Route
    name: "default",    // Part 1: If nothing is defined in the route, we should ...
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");     // go to the home controller -> find action index -> with possible ID

app.Run();      // Part 1: Finally Run the project


void SeedDatabase() {
    using (var scope = app.Services.CreateScope()) {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
