using Microsoft.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TamboliyaApi.GameLogic.Models;
using TamboliyaApi.GameLogic;
using TamboliyaApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<TamboliyaApi.Data.AppDbContext>(options => options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory,
    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddSingleton<UnitOfWork>();
builder.Services.AddScoped<Dodecahedron>();
builder.Services.AddScoped<Oracle>();
builder.Services.AddScoped<ChooseRandomActionService>();
builder.Services.AddScoped<NewMoveService>();
builder.Services.AddScoped<NewGame>();
builder.Services.AddScoped<LogService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy =>
    policy.WithOrigins("http://localhost:5000", "https://localhost:5001", "https://localhost:7147")
    .AllowAnyMethod()
    .WithHeaders(HeaderNames.ContentType));


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
