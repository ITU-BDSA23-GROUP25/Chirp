using Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Graph.Models.ExternalConnectors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "AzureADB2C";
})
.AddCookie()
.AddOAuth("GitHub", options =>
{
    options.ClientId = builder.Configuration["authentication:github:clientId"];
    options.ClientSecret = builder.Configuration["authentication:github:clientSecret"];
    options.CallbackPath = "/signin-github";
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.Run();
