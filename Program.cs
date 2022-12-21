using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;
using TamboliyaApi.Hubs;
using TamboliyaApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer4.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
builder.Services.Configure<IISOptions>(iis =>
{
    iis.AuthenticationDisplayName = "Windows";
    iis.AutomaticAuthentication = false;
});

// configures IIS in-proc settings
builder.Services.Configure<IISServerOptions>(iis =>
{
    iis.AuthenticationDisplayName = "Windows";
    iis.AutomaticAuthentication = false;
});


builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequiredLength = 7;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();




builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;

    options.UserInteraction = new UserInteractionOptions()
    {
        LogoutUrl = "/Account/Logout",
        LoginUrl = "/Account/Login",
        LoginReturnUrlParameter = "returnUrl",

    };
})
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.Ids)
    .AddInMemoryApiResources(Config.Apis)
    .AddInMemoryApiScopes(Config.GetApiScopes())
    .AddAspNetIdentity<AppUser>()
    .AddDeveloperSigningCredential();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);//работает для блейзор авторизации
builder.Services.AddAuthentication(/*работает для доступа к апи*/
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    )
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7212";
        options.Audience = "myApi";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            RoleClaimType = "role"
        };
    });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
    swagger.SchemaFilter<EnumSchemaFilter>();
    swagger.EnableAnnotations();
});

builder.Services.AddAutoMapper(typeof(Program));
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
    .AllowAnyHeader()
    .AllowCredentials()
    );


app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapDefaultControllerRoute();
app.Run();
