using IdentityModel.OidcClient;
using Training.Mobile.Settings;

namespace Training.Mobile.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAppSettings _appSettings;

    public IdentityService(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public async Task<ILoginResult> LoginAsync()        // The LoginResult provided by the client is wrapped in our own IdentityModelLoginResult and then returned
    {
        var options = new OidcClientOptions
        {
            Authority = _appSettings.OidcAuthority,
            ClientId = _appSettings.OidcClientId,
            ClientSecret = _appSettings.OidcClientSecret,
            Scope = _appSettings.OidcScope,
            RedirectUri = _appSettings.OidcRedirectUri,
            Browser = new WebAuthenticatorBrowser()
        };

        var client = new OidcClient(options);
#if DEBUG
        client.Options.Policy.Discovery.RequireHttps = false;
#endif
        try
        {
            LoginResult result = await client.LoginAsync(new LoginRequest());
            return new IdentityModelLoginResult(result);

        }
        catch (Exception e)
        {
            return new IdentityModelLoginResult("Unexpected error", e.Message);
        }
    }

    public async Task LogoutAsync(string identityToken)
    {
        var options = new OidcClientOptions
        {
            Authority = _appSettings.OidcAuthority,
            ClientId = _appSettings.OidcClientId,
            ClientSecret = _appSettings.OidcClientSecret,
            Scope = _appSettings.OidcScope,
            RedirectUri = _appSettings.OidcRedirectUri,
            PostLogoutRedirectUri = _appSettings.PostLogoutRedirectUri,
            Browser = new WebAuthenticatorBrowser()
        };
        var client = new OidcClient(options);
#if DEBUG
        client.Options.Policy.Discovery.RequireHttps = false;
#endif
        try
        {
            var logoutResult = await client.LogoutAsync(new LogoutRequest { IdTokenHint = identityToken });
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}