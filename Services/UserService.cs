using Microsoft.AspNetCore.Identity;
using TamboliyaApi.Data;
using TamboliyaLibrary.DAL;

namespace TamboliyaApi.Services
{
	public class UserService : IUserService
	{
		private AppDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;


		public UserService(AppDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task<ApplicationUser?> Authenticate(string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
				return null;

			var user = await _userManager.FindByEmailAsync(email);

			// check if username exists
			if (user == null)
				return null;

			bool passwordIsCorrect = await _userManager.CheckPasswordAsync(user, password);

			if (!passwordIsCorrect)
				return null;

			// authentication successful
			return user;
		}

		public IEnumerable<ApplicationUser> GetAll()
		{
			return _context.Users;
		}

		public ApplicationUser? GetById(int id)
		{
			return _context.Users.Find(id);
		}

		public async Task<IdentityResult> Create(string email, string password)
		{
			if (string.IsNullOrWhiteSpace(password))
				throw new ArgumentException("Password is required");
			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentException("Email is required");

			var userExists = await _userManager.FindByNameAsync(email);
			if (userExists != null)
				throw new ArgumentException("User already exists!");

			ApplicationUser user = new ApplicationUser()
			{
				Email = email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = email
			};

			var result = await _userManager.CreateAsync(user, password);
			if (!result.Succeeded)
				throw new InvalidOperationException("User creation failed! Please check user details and try again.");

			return result;
		}



	}
}
