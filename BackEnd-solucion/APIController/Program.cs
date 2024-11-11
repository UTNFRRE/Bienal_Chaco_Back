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
using FluentValidation;
using APIController.Validadores;

var builder = WebApplication.CreateBuilder(args);

// Cargar las configuraciones de Azure Blob Storage desde appsettings
//crear variable para cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("Connection");

// Configurar la conexión a la base de datos inMemory
//builder.Services.AddDbContext<BienalDbContext>(options => options.UseInMemoryDatabase("PruebaBD"));
builder.Services.AddDbContext<BienalDbContext>(options => options.UseSqlServer(connectionString,
     b => b.MigrationsAssembly("APIController")));

builder.Services.AddScoped<IAzureStorageService, AzureBlobStorageService>();            

builder.Services.AddScoped<ICRUDEsculturaService, EsculturasServices>();
builder.Services.AddScoped<ICRUDServiceEvent, EventosServices>();
builder.Services.AddScoped<ICRUDServicesEscultores, EscultoresServices>();

builder.Services.AddScoped<ICRUDServicesVotos, VotosService>();


builder.Services.AddScoped<ICRUDServiceEdicion, EdicionServices>();

// 
// Add Identity services
builder.Services.AddIdentity<MyUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;

    // Email settings
    options.SignIn.RequireConfirmedEmail = false;

    // Lockout settings
    options.Lockout.AllowedForNewUsers = false;
    options.Lockout.MaxFailedAccessAttempts = 12;
})
.AddEntityFrameworkStores<BienalDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<ICRUDServiceUsers, UsersServices>();

builder.Services.AddValidatorsFromAssemblyContaining<EdicionValidator>();

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bienal API", Version = "V1.1" });
});

builder.Services.AddCors();

var app = builder.Build();

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
                        }
            );

app.UseAuthorization();

app.MapControllers();

app.Run();
