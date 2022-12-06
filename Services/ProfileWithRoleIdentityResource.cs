using IdentityServer4.Models;
using IdentityModel;

namespace TamboliyaApi.Services
{
    public class ProfileWithRoleIdentityResource : IdentityResources.Profile
    {
        public ProfileWithRoleIdentityResource()
        {
            this.UserClaims.Add(JwtClaimTypes.Role);
        }
    }
}
