using System.Diagnostics;
using IdentityModel.Client;
using IdentityModel.OidcClient.Browser;

namespace Training.Mobile.Services.Identity;

internal class WebAuthenticatorBrowser : IdentityModel.OidcClient.Browser.IBrowser
{
    public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)     // Sends the user to a browser that shows the identity provider's login page
    {
        try
        {
            Uri startUrl = new Uri(options.StartUrl);
            Uri endUrl = new Uri(options.EndUrl);

            WebAuthenticatorResult authResult = await WebAuthenticator.AuthenticateAsync(startUrl, endUrl);

            var callbackUrl = new RequestUrl(options.EndUrl).Create(new Parameters(authResult.Properties));

            return new BrowserResult
            {
                Response = callbackUrl,
                ResultType = BrowserResultType.Success
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return new BrowserResult()
            {
                ResultType = BrowserResultType.UnknownError,
                Error = ex.ToString()
            };
        }
    }
}