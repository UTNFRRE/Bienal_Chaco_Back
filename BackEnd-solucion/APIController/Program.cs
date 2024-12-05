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
using Servicios.Identity;


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

builder.Services.AddAuthorization();


builder.Services.AddScoped<IAzureStorageService, AzureBlobStorageService>();            

builder.Services.AddScoped<ICRUDEsculturaService, EsculturasServices>();
builder.Services.AddScoped<ICRUDServiceEvent, EventosServices>();
builder.Services.AddScoped<ICRUDServicesEscultores, EscultoresServices>();
builder.Services.AddScoped<ICRUDServicesVotos, VotosService>();
builder.Services.AddScoped<ICRUDServiceEdicion, EdicionServices>();

builder.Services.AddScoped<IRolesServices, RolesServices>();

builder.Services.AddScoped<IServiceUsers, UsersServices>();

//builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

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


builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

//connfiguracion de Header con el Bearer Token en Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bienal API", Version = "V1.1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
        });
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

app.UseAuthentication();
app.UseAuthorization();

//configuracion de Cors
app.UseCors(options => { 
                        options.AllowAnyOrigin();
                        options.AllowAnyMethod();
                        options.AllowAnyHeader();
                        }
            );

app.MapControllers();

app.Run();
