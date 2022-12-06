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


        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                // the api requires the role claim
                new ApiResource("tamboliyaApi", "The Tamboliya API", new[] { JwtClaimTypes.Role })
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
                    AllowedScopes = { "openid", "profile", "email" },
                    RedirectUris = { "https://localhost:7147/authentication/login-callback" },
                    PostLogoutRedirectUris = { "https://localhost:7147/authentication/logout-callback" },
                    Enabled = true
                },
            };
    }
}
