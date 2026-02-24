using HazelNet_Application.Auth;
using HazelNet_Application.Interface;
using HazelNet_Infrastracture.Command;
using MudBlazor.Services;
using HazelNet_Web.Core;
using HazelNet_Infrastracture.DBContext;
using HazelNet_Web.Features.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;

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
app.UseAntiforgery();
app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

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
       return Results.BadRequest(new { error = "Email exists" });
});

app.MapPost("/login", async (
    HttpContext httpContext,
    LoginHandler handler,
    Login.RegisterAccountForm model) =>
{
    
    var command = new LoginHandler.LoginQuery(model.Email, model.Password);
    var result = await handler.Handle(command);

    if (result.Success)
        return Results.Ok(new { success = true });
    else
        return Results.BadRequest(new { error = "Wrong credentials" });
});


app.MapGet("/logout", async (HttpContext HttpContext) =>
{
    await HttpContext.SignOutAsync();
    return Results.Redirect("/login");
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
