using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using front_gestionnaire_password.Components;


var builder = WebApplication.CreateBuilder(args);

string apiEndpoint = builder.Configuration.GetValue<string>("WebAPI:Endpoint") ?? throw new InvalidOperationException("WebAPI is not configured");
string apiScope = builder.Configuration.GetValue<string>("WebAPI:Scope") ?? throw new InvalidOperationException("WebAPI is not configured");

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration, "AzureAd")
        .EnableTokenAcquisitionToCallDownstreamApi([apiScope])
            .AddDownstreamApi("EntraIDAuthWebAPI", options =>
            {
                options.BaseUrl = apiEndpoint;
                options.Scopes = [apiScope];
            })
    .AddInMemoryTokenCaches();
/*
builder.Services.AddDownstreamApi(
    "EntraIDAuthWebAPI",
    builder.Configuration.GetSection("DownstreamApi")
);
*/

builder.Services.AddAuthorization();

// --- Blazor Components ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

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
    .RequireAuthorization()
    .AddInteractiveServerRenderMode();

app.Run();