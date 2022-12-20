using IdentityServer4.Models;
using IdentityModel;

namespace TamboliyaApi.Services
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                //new IdentityResources.Profile(),
                new ProfileWithRoleIdentityResource(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(name: "tamboliya-Api",   displayName: "The Tamboliya API", new[] { JwtClaimTypes.Role }),
                new ApiScope("myApi.read"),
                new ApiScope("myApi.write")
            };
        }

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                // the api requires the role claim
                new ApiResource("tamboliya-Api", "The Tamboliya API", new[] { JwtClaimTypes.Role }),
                new ApiResource("myApi")
                {
                    Scopes = new List<string>{ "myApi.read","myApi.write" },
                    ApiSecrets = new List<Secret>{ new Secret("supersecret".Sha256()) }
                }
            };


        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "blazorWASM",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowedCorsOrigins = { "https://localhost:7147" },
                    AllowedScopes = { "openid", "profile", "email", "tamboliya-Api" },
                    RedirectUris = { "https://localhost:7147/authentication/login-callback" },
                    PostLogoutRedirectUris = { "https://localhost:7147/authentication/logout-callback" },
                    Enabled = true
                },
                new Client
                {
                    ClientId = "cwm.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes = { "myApi.read" }
                },
            };
    }
}
