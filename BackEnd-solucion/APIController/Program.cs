using Azure.Storage.Blobs; // Paquete de Azure.Storage.Blobs necesario
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Contexts;
using Servicios;
using Entidades;
using Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using static Servicios.Ediciones;


// Add these using statements at the top
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Azure.Core;


var builder = WebApplication.CreateBuilder(args);

// Cargar las configuraciones de Azure Blob Storage desde appsettings
//crear variable para cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("Connection");


// Configurar la conexión a la base de datos inMemory
//builder.Services.AddDbContext<BienalDbContext>(options => options.UseInMemoryDatabase("PruebaBD"));
builder.Services.AddDbContext<BienalDbContext>(options => options.UseSqlServer(connectionString,
     b => b.MigrationsAssembly("APIController")));

// Configurar MyIdentityDBContext
builder.Services.AddDbContext<MyIdentityDBContext>(options => options.UseSqlServer(connectionString,
     b => b.MigrationsAssembly("APIController")));

builder.Services.AddAuthentication()
                                    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddAuthorizationBuilder();


builder.Services.AddScoped<IAzureStorageService, AzureBlobStorageService>();            

builder.Services.AddScoped<ICRUDEsculturaService, EsculturasServices>();
builder.Services.AddScoped<ICRUDServiceEvent, EventosServices>();
builder.Services.AddScoped<ICRUDServicesEscultores, EscultoresServices>();
builder.Services.AddScoped<ICRUDServicesVotos, VotosService>();
builder.Services.AddScoped<ICRUDServiceEdicion, EdicionServices>();

//builder.Services.AddScoped<IServiceUsers, UsersServices>();

//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

//builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


// Configurar IdentityCore
builder.Services.AddIdentity<MyUser, MyRol>(options => {
    
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 999;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;
})                                                      
    .AddEntityFrameworkStores<MyIdentityDBContext>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

// Configurar IdentityOptions

/*
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
*/
// Configure Authorization
//builder.Services.AddAuthorization();

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bienal API", Version = "V1.1" });
});

builder.Services.AddCors();

var app = builder.Build();

app.MapIdentityApi<MyUser>();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bienal API V1.1");
    });
}

app.UseHttpsRedirection();
app.UseCors(options => { 
                        options.AllowAnyOrigin();
                        options.AllowAnyMethod();
                        options.AllowAnyHeader();
                        }
            );

app.MapControllers();

//endpoint para devolver la informacion del usuario en la sesion
app.MapGet("users/info", async (ClaimsPrincipal claims, MyIdentityDBContext context) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

    var userLogueado = await context.Users.FindAsync(userId);

    if (userLogueado == null)
    {
        return Results.NotFound("User not found");
    }

    return Results.Ok(new
    {
        Id = userLogueado.Id,
        UserName = userLogueado.UserName,
        Email = userLogueado.Email
    });
});

//endpoint lista de usuarios 
app.MapGet("users/{email}", async (string UserName, MyIdentityDBContext context) =>
{
    var userLogueado = await context.Users.FirstOrDefaultAsync(user => user.UserName == UserName);

    if (userLogueado == null)
    {
        return Results.NotFound("User not found");
    }

    return Results.Ok(new
    {
        Id = userLogueado.Id,
        UserName = userLogueado.UserName,
        Email = userLogueado.Email
    });
});

app.Run();
