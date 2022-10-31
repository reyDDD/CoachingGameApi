using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;
using TamboliyaApi.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Identity;
using TamboliyaApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
	 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
	swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
	$"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
	swagger.SchemaFilter<EnumSchemaFilter>();
	swagger.EnableAnnotations();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
	{
		options.SignIn.RequireConfirmedAccount = false;
	})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();


// Adding Authentication  
var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]);

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	
	options.Events = new JwtBearerEvents
	{
		OnTokenValidated = context =>
		{
			var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
			var userId = int.Parse(context.Principal.Identity.Name);
			var user = userService.GetById(userId);
			if (user == null)
			{
				// return unauthorized if user no longer exists
				context.Fail("Unauthorized");
			}
			return Task.CompletedTask;
		}
	};
	options.RequireHttpsMetadata = false;
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(key),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});



builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream" });
});


builder.Services.AddSingleton<ProphecyCollectionService>();
builder.Services.AddSingleton<RandomService>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<Dodecahedron>();
builder.Services.AddScoped<Oracle>();
builder.Services.AddScoped<ChooseRandomActionService>();
builder.Services.AddScoped<NewMoveService>();
builder.Services.AddScoped<NewGame>();
builder.Services.AddScoped<LogService>();
builder.Services.AddSingleton<PositionsOnMapService>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

app.UseResponseCompression();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors(policy =>
	policy.WithOrigins("http://localhost:5000", "https://localhost:5001", "https://localhost:7147", "https://localhost:7112")
	.AllowAnyMethod()
	//.WithHeaders(HeaderNames.ContentType))
	.AllowAnyHeader()
	.AllowCredentials());


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.Run();
