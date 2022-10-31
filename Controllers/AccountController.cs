using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TamboliyaApi.Data;
using TamboliyaApi.Services;
using TamboliyaLibrary.DAL;

namespace TamboliyaApi.Controllers
{
	[AllowAnonymous]
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogger<AccountController> _logger;
		private readonly IConfiguration _configuration;
		private readonly IUserService _userService;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<AccountController> logger,
			IConfiguration configuration,
			IUserService userService)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_logger = logger;
			_userService = userService;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model, string userRoles)
		{
			ApplicationUser user = new ApplicationUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Email
			};

			try
			{
				await _userService.Create(model.Email, model.Password);
				_logger.LogInformation("There is added a new user with password.");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			var userIsAdmin = User.IsInRole("Admin");

			if (await _roleManager.RoleExistsAsync(UserRoles.User) && !userIsAdmin)
			{
				await _userManager.AddToRoleAsync(user, UserRoles.User);
			}
			else if (await _roleManager.RoleExistsAsync(UserRoles.Admin) && userIsAdmin)
			{
				await _userManager.AddToRoleAsync(user, UserRoles.Admin);
			}

			return Ok("User created successfully!");
		}



		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] RegisterModel model)
		{

			var user = await _userService.Authenticate(model.Email, model.Password);

			if (user == null)
				return new BadRequestObjectResult("Email or password is incorrect");

			var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);


			List<Claim> claims = new List<Claim>()
			{
				new Claim(ClaimTypes.Email, user.Email)
			};

			var userRoles = await _userManager.GetRolesAsync(user);
			foreach (var userRole in userRoles)
			{
				claims.Add(new Claim(ClaimTypes.Role, userRole));
			}

			var tokenHandler = new JwtSecurityTokenHandler();

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			// return basic user info and authentication token
			return Ok(new
			{
				Id = user.Id,
				Email = user.Email,
				Token = tokenString,
				Expiration = token.ValidTo
			});
		}
	}
}
