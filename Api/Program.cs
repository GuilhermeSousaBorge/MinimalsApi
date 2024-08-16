using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.DTOs;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.Interfaces;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.DTO;
using MinimalApi.Infrastructure;


#region Builder
var builder = WebApplication.CreateBuilder(args);
var Key = builder.Configuration.GetSection("Jwt").ToString();

if (string.IsNullOrEmpty(Key)) Key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),
        ValidateIssuer = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insert your JWT token in the text box below! Like Bearer: {token}",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<Dbcontext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"), ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});
var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administrators

string GenerateJwtToken(Administrator administrator)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Role", administrator.Role),
        new Claim(ClaimTypes.Role, administrator.Role)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/Administradores/login", ([FromBody] LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);
    if (adm != null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new AdministratorLogged
        {
            Email = adm.Email,
            Role = adm.Role,
            Token = token
        });
    }
    return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapPost("/Administradores", ([FromBody] AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validations = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email))
    {
        validations.Messages.Add("O Email não pode ser vazio");
    }

    if (string.IsNullOrEmpty(administratorDTO.Password))
    {
        validations.Messages.Add("A senha não pode ser vazia");
    }

    if (administratorDTO.Role == null)
    {
        validations.Messages.Add("O Perfil não pode ser vazio");
    }

    if (validations.Messages.Count > 0)
    {
        return Results.BadRequest(validations);
    }

    var administrators = new Administrator
    {
        Email = administratorDTO.Email,
        Password = administratorDTO.Password,
        Role = administratorDTO.Role.ToString() ?? Role.Editor.ToString()
    };

    administratorService.Save(administrators);
    return Results.Created($"/administrador/{administrators.Id}", new AdministratorModelView
    {
        Id = administrators.Id,
        Email = administrators.Email,
        Role = administrators.Role
    });
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Administradores");

app.MapGet("/administradores", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();

    var administrators = administratorService.AllAdministrators(page);

    foreach (var adm in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Role = adm.Role
        });
    }
    return Results.Ok(administrators);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.FindById(id);
    if (administrator == null) return Results.NotFound();
    return Results.Ok(new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Role = administrator.Role
    });
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Administradores");

#endregion

ValidationErrors validation(VehicleDTO vehicleDTO)
{
    var errorMesages = new ValidationErrors
    {
        Messages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Name))
    {
        errorMesages.Messages.Add("O Nome não pode ser vazio");
    }
    if (string.IsNullOrEmpty(vehicleDTO.Model))
    {
        errorMesages.Messages.Add("O Model não pode ser vazio");
    }
    if (vehicleDTO.Ano == 0)
    {
        errorMesages.Messages.Add("O Ano não pode ser 0");
    }
    return errorMesages;
}

#region Vehicles
app.MapPost("/veiculos", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{

    var validations = validation(vehicleDTO);

    if (validations.Messages.Count > 0)
    {
        return Results.BadRequest(validations);
    }

    var vehicle = new Vehicles
    {
        Name = vehicleDTO.Name,
        Model = vehicleDTO.Model,
        Ano = vehicleDTO.Ano
    };
    vehicleService.Save(vehicle);
    return Results.Created($"/veiculo/{vehicle.Id}", vehicle);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.AllVehicles(page);
    return Results.Ok(vehicles);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();
    return Results.Ok(vehicle);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm, Editor" }).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    var validations = validation(vehicleDTO);

    if (validations.Messages.Count > 0)
    {
        return Results.BadRequest(validations);
    }

    vehicle.Name = vehicleDTO.Name;
    vehicle.Model = vehicleDTO.Model;
    vehicle.Ano = vehicleDTO.Ano;

    vehicleService.Update(vehicle);
    return Results.Ok(vehicle);
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" }).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.FindById(id);
    if (vehicle == null) return Results.NotFound();

    vehicleService.Delete(vehicle);
    return Results.NoContent();
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" }).WithTags("Veiculos");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion