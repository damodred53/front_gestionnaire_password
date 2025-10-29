using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Identity.Web;

namespace front_gestionnaire_password.Components.Pages;

public partial class Home : ComponentBase
{
    // [Inject]
    // public IDownstreamWebApi DownstreamApi { get; set; }
    [Inject]
    public AuthenticationStateProvider AuthenticationStateProvider {get; set;}
    [Inject]
    public NavigationManager NavigationManager { get; set; }
    [Inject]
    ITokenAcquisition TokenAcquisition {get; set;}
    private ClaimsPrincipal? _user;
    private string? _token;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _user = authState.User;
        
        // On ne tente d’obtenir le token que si l’utilisateur est bien authentifié
        if (_user?.Identity?.IsAuthenticated == true)
        {
            try
            {
                _token = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { "User.Read" });
                Console.WriteLine(_token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du token : {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Utilisateur non authentifié — impossible de récupérer le token.");
        }
    }
}