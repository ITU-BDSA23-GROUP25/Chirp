using System.Configuration;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

/*
 * The following code configures and sets up an ASP.NET Core web application using the builder pattern.
 * It includes the configuration of the database, services, authentication providers, and middleware.
 */

// Create a builder for the web application
var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from the sql server
var connectionString = builder.Configuration["ConnectionString"] ?? throw new InvalidOperationException("Connection string 'DatabaseContextConnection' not found.");

// Add Entity Framework DbContext to the service container with SQL Server as the database provider
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

// Add Razor Pages services to the container
builder.Services.AddRazorPages();

// Add repositories to the dependency injection container
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();

// Configure authentication services
builder.Services.AddAuthentication()
    .AddGitHub("GitHub", o =>
    {
        o.ClientId = builder.Configuration["Github:ClientId"];
        o.ClientSecret = builder.Configuration["Github:ClientSecret"];
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email"); // Add additional scopes if needed
        o.Scope.Add("user:name");
        o.Scope.Add("user:id");

        o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        o.TokenEndpoint = "https://github.com/login/oauth/access_token";
        o.UserInformationEndpoint = "https://api.github.com/user";

        o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
    });


// Add Identity services for Author entities to the service container
builder.Services.AddDefaultIdentity<Author>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<DatabaseContext>();

// Build the web application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Enable HTTPS redirection and use static files
app.UseHttpsRedirection();
app.UseStaticFiles();

// Configure routing and enable authentication and authorization
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages in the application
app.MapRazorPages();

//start program
app.Run();


