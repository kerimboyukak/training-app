using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IdentityModel.OidcClient;

namespace KWops.Cli
{
    class OidcService
    {
        public async Task<string> GetAccessTokenAsync()
        {
            //https://identitymodel.readthedocs.io/en/latest/native/manual.html

            string redirectUri = "http://localhost:7890/";
            var options = new OidcClientOptions
            {
                Authority = "https://localhost:9001",
                ClientId = "kwops.cli",
                ClientSecret = "SuperSecretClientSecret",
                Scope = "openid profile devops.read",
                RedirectUri = redirectUri
            };

            var client = new OidcClient(options);

            var state = await client.PrepareLoginAsync();

            OpenBrowser(state.StartUrl);

            Console.WriteLine($"\n\nLogin via the browser window that just opened at '{options.Authority}'. \nWhen you are redirected to '{redirectUri}', paste the querystring of the url you see in the address bar. Then press enter:");
            string data = Console.ReadLine();

            var result = await client.ProcessResponseAsync(data, state);

            if (result.IsError)
            {
                Console.WriteLine("\n\nError:\n{0}", result.Error);
                return string.Empty;
            }

            Console.WriteLine("\n\nClaims:");
            foreach (var claim in result.User.Claims)
            {
                Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            Console.WriteLine();
            Console.WriteLine("Access token:\n{0}", result.AccessToken);

            if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                Console.WriteLine("Refresh token:\n{0}", result.RefreshToken);
            }

            return result.AccessToken;
        }

        private static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}