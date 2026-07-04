using dotnetcoresample.Components;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Entra ID (Azure AD) authentication
builder.Services.AddAuthentication(Microsoft.Identity.Web.Constants.AzureADOpenID)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Needed so Blazor components can see the auth state
builder.Services.AddCascadingAuthenticationState();

// Microsoft.Identity.Web.UI provides the /MicrosoftIdentity/Account/SignIn
// and SignOut MVC endpoints — Blazor itself has no login UI, so we still
// need controllers for that part.
builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers(); // exposes the sign-in/out endpoints
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
