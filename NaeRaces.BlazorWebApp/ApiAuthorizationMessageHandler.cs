using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace NaeRaces.BlazorWebApp;

class ApiAuthorizationMessageHandler : AuthorizationMessageHandler
{
    public ApiAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigation, string[] authorizedUrls) : base(provider, navigation)
    {
        ConfigureHandler(authorizedUrls: authorizedUrls);
    }
}
