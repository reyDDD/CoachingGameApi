using Microsoft.AspNetCore.Identity;
using TamboliyaApi.Data;

namespace TamboliyaApi.Services
{
	public interface IUserService
	{
		Task<ApplicationUser?> Authenticate(string email, string password);
		IEnumerable<ApplicationUser> GetAll();
		ApplicationUser? GetById(string? id);
		Task<IdentityResult> Create(string email, string password);
	}
}
