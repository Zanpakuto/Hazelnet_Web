using System.Security.Claims;
using HazelNet_Application.Auth;
using HazelNet_Application.Interface;
using HazelNet_Infrastracture.Command;
using MudBlazor.Services;
using HazelNet_Web.Core;
using HazelNet_Infrastracture.DBContext;
using HazelNet_Web.Features.Account;
using HazelNet_Web.Features.Private;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>( option =>
    option.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "HazelNet.Auth";
        opt.LoginPath = "/Login";
        opt.AccessDeniedPath = "/Forbidden";
        opt.ExpireTimeSpan = TimeSpan.FromMinutes(45);
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();



builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddHttpClient();


builder.Services.AddHttpClient("LocalApi", (sp, client) =>
{
    var navManager = sp.GetRequiredService<NavigationManager>();
    client.BaseAddress = new Uri(navManager.BaseUri);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();     



//Form Post for login and auth validation
//Implement later
app.MapPost("/register", async (
    HttpContext httpContext,
    RegisterHandler handler,
    AccountSignUp.RegisterAccountForm model) =>
{
    
   var command = new RegisterHandler.RegisterUserCommand(model.Username, model.Email, model.Password);
   var result = await handler.Handle(command);

   if (result.Success)
       return Results.Ok(new { success = true });
   else
       return Results.BadRequest(new { error = "Email already exists" });
});

app.MapPost("/login", async (
    HttpContext httpContext,
    [FromServices]LoginHandler handler,
    [FromForm] string email,
    [FromForm] string password) =>
{
    
    var command = new LoginHandler.LoginQuery(email, password);
    var result = await handler.Handle(command);

    if (result.Success)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, "User")
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        

        await httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            });


        return Results.Redirect("/Dashboard");
    }
    else 
        return Results.Redirect("/login?error=true");
});


app.MapGet("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
