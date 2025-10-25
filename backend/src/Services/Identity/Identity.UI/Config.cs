using Duende.IdentityServer.Models;

namespace Identity.UI;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("devops.read", "Read access on DevOps Api"),
            new ApiScope("hr.read", "Read access on HumanResources Api"),
            new ApiScope("training.read", "Read access on Training Api"),

            new ApiScope("manage", "Write access")
        };
    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        {
            new ApiResource("devops", "DevOps API")
            {
                Scopes = { "devops.read", "manage" },

            },
            new ApiResource("hr", "Human Resources API")
            {
                Scopes = { "hr.read", "manage" }
            },
            new ApiResource("training", "Training API")
            {
                Scopes = { "training.read", "manage" }
            }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client
            {
                ClientId = "kwops.cli",
                ClientName = "KWops Command Line Interface",
                ClientSecrets = { new Secret("SuperSecretClientSecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "http://localhost:7890/" },
                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "devops.read", "hr.read", "training.read" }
            },
            new Client
            {
                ClientId = "swagger.devops",
                ClientName = "Swagger UI for DevOps Api",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RedirectUris = {"https://localhost:8001/swagger/oauth2-redirect.html"},
                AllowedCorsOrigins = {"https://localhost:8001"},
                AllowedScopes = {"devops.read", "manage"}
            },
            new Client
            {
                ClientId = "swagger.hr",
                ClientName = "Swagger UI for Human Resources Api",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RedirectUris = {"https://localhost:5001/swagger/oauth2-redirect.html"},
                AllowedCorsOrigins = {"https://localhost:5001"},
                AllowedScopes = {"hr.read", "manage"}
            },
            new Client
            {
                ClientId = "swagger.training",
                ClientName = "Swagger UI for Training Api",
                AllowedGrantTypes = GrantTypes.Code,
                RequireClientSecret = false,
                RedirectUris = {"https://localhost:7001/swagger/oauth2-redirect.html"},
                AllowedCorsOrigins = {"https://localhost:7001"},
                AllowedScopes = {"training.read", "manage"}
            },
            new Client
            {
                ClientId = "kwops.mobile",
                ClientName = "KWops mobile application",
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("MobileClientSecret".Sha256()) },
                RedirectUris = {"myapp://mauicallback"},
                AllowedScopes = {"openid", "devops.read", "manage"},
                PostLogoutRedirectUris = {"myapp://mauilogout"}
            },
             new Client
            {
                ClientId = "training.mobile",
                ClientName = "Training mobile application",
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("MobileClientSecret".Sha256()) },
                RedirectUris = {"myapp://mauicallback"},
                AllowedScopes = {"openid", "training.read", "manage"},
                PostLogoutRedirectUris = {"myapp://mauilogout"}
            }

        };
}
