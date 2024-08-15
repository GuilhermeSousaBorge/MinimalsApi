using Microsoft.EntityFrameworkCore;
using MinimalApi.DTO;
using MinimalApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<Dbcontext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/login", (LoginDTO loginDTO) =>
{
    if (loginDTO.Email == "admin@teste.com" && loginDTO.Password == "123456") return Results.Ok("Login efetuado com sucesso!");
    return Results.Unauthorized();
});

app.Run();
