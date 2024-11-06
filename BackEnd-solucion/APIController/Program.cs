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

var builder = WebApplication.CreateBuilder(args);

// Cargar las configuraciones de Azure Blob Storage desde appsettings
//crear variable para cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("Connection");
builder.Services.AddDbContext<BienalDbContext>(options => options.UseSqlServer(connectionString,
     b => b.MigrationsAssembly("APIController")));

builder.Services.AddScoped<IAzureStorageService, AzureBlobStorageService>();            

builder.Services.AddScoped<ICRUDEsculturaService, EsculturasServices>();
builder.Services.AddScoped<ICRUDServiceEvent, EventosServices>();
builder.Services.AddScoped<ICRUDServicesEscultores, EscultoresServices>();

// 
// Add Identity services
builder.Services.AddIdentity<MyUser, MyRol>(options =>
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
.AddEntityFrameworkStores<MyIdentityDBContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<ICRUDServiceUsers, UsersServices>();

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
