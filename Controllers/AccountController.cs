using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TamboliyaApi.Data;
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

		public AccountController(
			UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			ILogger<AccountController> logger,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_configuration = configuration;
			_logger = logger;
		}

		[HttpPost]
		public async Task<IActionResult> Register(string email, string password)
		{
			if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(password))
			{
				var user = new ApplicationUser { UserName = email, Email = email };
				var result = await _userManager.CreateAsync(user, password);
				if (result.Succeeded)
				{
					_logger.LogInformation("There is added a new user with password.");


				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return Ok("token");
			}
			else
			{
				return Ok("token");
			}

		}

		[Authorize(Roles = UserRoles.Admin)]
		[HttpPost]
		[Route("register-user")]
		public async Task<IActionResult> RegisterUser([FromBody] RegisterModel model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Email);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

			ApplicationUser user = new ApplicationUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Email
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			if (await _roleManager.RoleExistsAsync(UserRoles.User))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.User);
			}

			return Ok(new Response { Status = "Success", Message = "User created successfully!" });
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
		{
			var userExists = await _userManager.FindByNameAsync(model.Email);
			if (userExists != null)
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

			ApplicationUser user = new ApplicationUser()
			{
				Email = model.Email,
				SecurityStamp = Guid.NewGuid().ToString(),
				UserName = model.Email
			};
			var result = await _userManager.CreateAsync(user, model.Password);
			if (!result.Succeeded)
				return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

			if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
			if (!await _roleManager.RoleExistsAsync(UserRoles.User))
				await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

			if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
			{
				await _userManager.AddToRoleAsync(user, UserRoles.Admin);
			}

			return Ok(new Response { Status = "Success", Message = "User created successfully!" });
		}


		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] RegisterModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Email);
			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var userRoles = await _userManager.GetRolesAsync(user);

				var authClaims = new List<Claim>
				{
					new Claim(ClaimTypes.Name, user.UserName),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				};

				foreach (var userRole in userRoles)
				{
					authClaims.Add(new Claim(ClaimTypes.Role, userRole));
				}

				var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

				var token = new JwtSecurityToken(
					issuer: _configuration["JWT:ValidIssuer"],
					audience: _configuration["JWT:ValidAudience"],
					expires: DateTime.Now.AddHours(3),
					claims: authClaims,
					signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
					);

				return Ok(new
				{
					token = new JwtSecurityTokenHandler().WriteToken(token),
					expiration = token.ValidTo
				});
			}
			return Unauthorized();
		}

	}
}
