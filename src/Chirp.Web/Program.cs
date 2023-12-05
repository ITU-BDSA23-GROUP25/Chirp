using System.Configuration;
using Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

public class Program
{

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DatabaseContextConnection") ?? throw new InvalidOperationException("Connection string 'DatabaseContextConnection' not found.");

        builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
        builder.Services.AddAuthentication(options =>
    {
        //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "GitHub";
    })
    .AddCookie()
    .AddGitHub("GitHub", o =>
    {
        Console.WriteLine(Environment.GetEnvironmentVariable("GitHubClientId") + " ok");
        Console.WriteLine(Environment.GetEnvironmentVariable("GitHubClientSecret") + " ok");
        o.ClientId = "7152d1322e5745a15ba6";
        o.ClientSecret = "5829d1c2587afa42bc40254e6e958fb480b2553c";
        o.CallbackPath = "/signin-github";
        o.Scope.Add("user:email"); // Add additional scopes if needed
        o.Events = new OAuthEvents
    {
        OnCreatingTicket = context =>
        {
            // Log or print relevant information for debugging
            Console.WriteLine("OnCreatingTicket event fired.");
            return Task.CompletedTask;
        }
    };
    });

        builder.Services.AddDefaultIdentity<Author>(options =>
        options.SignIn.RequireConfirmedAccount = true)
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

    }

}
