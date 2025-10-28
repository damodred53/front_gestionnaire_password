using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using front_gestionnaire_password.Components;

var builder = WebApplication.CreateBuilder(args);

// --- 🔐 Authentification Microsoft Entra ID ---
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.AddAuthorization(options =>
{
    // Tout utilisateur connecté a accès sauf si précisé
    options.FallbackPolicy = options.DefaultPolicy;
});

// --- Blazor Components ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler(); // Important pour gérer les consentements

var app = builder.Build();

// --- Pipeline HTTP ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🔑 Anti-forgery doit venir APRES auth, AVANT les endpoints
app.UseAntiforgery();

// --- Blazor ---
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

/*app.MapPost("/logout", async context =>
{
    // Déconnexion côté application (cookie)
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Déconnexion côté Azure AD (OIDC)
    await context.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

    // Redirection après déconnexion
    context.Response.Redirect("/");
});*/

app.Run();