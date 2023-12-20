using System.Configuration;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionString"] ?? throw new InvalidOperationException("Connection string 'DatabaseContextConnection' not found.");

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IFollowerRepository, FollowerRepository>();
builder.Services.AddScoped<IReactionRepository, ReactionRepository>();

builder.Services.AddAuthentication()
    .AddGitHub("GitHub", o =>
    {
        o.ClientId = builder.Configuration["Github:ClientId"];
        o.ClientSecret = builder.Configuration["Github:ClientSecret"];
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email"); // Add additional scopes if needed
        o.Scope.Add("user:name");
        o.Scope.Add("user:id");
        //o.ClaimActions.MapJsonKey("user:email", "Email", "email");

        o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        o.TokenEndpoint = "https://github.com/login/oauth/access_token";
        o.UserInformationEndpoint = "https://api.github.com/user";

        o.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        o.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        o.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                // Log claims during the authentication process
                foreach (var claim in context.Principal.Claims)
                {
                    Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
                }

                return Task.CompletedTask;
            }
        };
    });



builder.Services.AddDefaultIdentity<Author>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<DatabaseContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//app.UseSession();

app.MapRazorPages();
app.Run();


