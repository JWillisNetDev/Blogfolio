using Blogfolio.Server.Components;

using Blogfolio.Data;
using MudBlazor.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Blogfolio.Server.Identity;
using Microsoft.AspNetCore.Identity;
using Blogfolio.Data.Identity;
using System.Reflection.Metadata;
using Blogfolio.Server;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddMudServices()
    .AddBlogfolioDb(builder.Configuration);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingAuthenticationStateProvider>();

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddIdentityCookies();

builder.Services
    .AddIdentityCore<BlogfolioUser>(opts =>
    {
        opts.SignIn.RequireConfirmedAccount = false;
        opts.User.RequireUniqueEmail = true;
        opts.Password.RequiredLength = 8;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlogfolioDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    using var db = await services.GetRequiredService<IDbContextFactory<BlogfolioDbContext>>().CreateDbContextAsync();
    db.Database.EnsureCreated();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync(Constants.Roles.Administrator))
    {
        IdentityRole adminRole = new(Constants.Roles.Administrator);
        await roleManager.CreateAsync(adminRole);
        await roleManager.AddClaimAsync(adminRole, new Claim(Constants.Claims.DeleteAnyTodo, "true"));
    }

    var userManager = services.GetRequiredService<UserManager<BlogfolioUser>>();
    var user = await userManager.FindByEmailAsync("administrator@blogfolio.com");
    if (user is null)
    {
        user = new BlogfolioUser()
        {
            UserName = "administrator@blogfolio.com",
            Email = "administrator@blogfolio.com",
            EmailConfirmed = true,
        };
        await userManager.CreateAsync(user, "Developer123!");
    }
    
    if (!await userManager.IsInRoleAsync(user, Constants.Roles.Administrator))
    {
        await userManager.AddToRoleAsync(user, Constants.Roles.Administrator);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/account/logout", async (SignInManager<BlogfolioUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return TypedResults.LocalRedirect("/");
});

app.Run();
