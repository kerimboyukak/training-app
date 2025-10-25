using System;

namespace KWops.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("|  Sign in with OIDC    |");
            Console.WriteLine("+-----------------------+");
            Console.WriteLine("");
            Console.WriteLine("Press any key to sign in...");
            Console.ReadKey(true);

            var tokenService = new OidcService();
            string accessToken = tokenService.GetAccessTokenAsync().Result;

            Console.WriteLine("\n\nRetrieving teams from DevOps microservice...");
            var devOpsApiClient = new DevOpsApiClient();
            string teams = devOpsApiClient.GetTeamsAsJsonAsync(accessToken).Result;
            Console.WriteLine(teams);

            Console.WriteLine("Press any key to close...");
            Console.ReadKey(true);
        }
    }
}
