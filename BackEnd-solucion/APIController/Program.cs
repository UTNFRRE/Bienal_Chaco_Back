using Azure.Storage.Blobs; // Paquete de Azure.Storage.Blobs necesario
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Cargar las configuraciones de Azure Blob Storage desde appsettings
var azureBlobOptions = builder.Configuration.GetSection("AzureBlobStorage").Get<AzureBlobOptions>();

// Registrar el cliente de Azure Blob Storage con la cadena de conexión desde appsettings
builder.Services.AddSingleton<BlobServiceClient>(sp =>
{
    return new BlobServiceClient(azureBlobOptions.ConnectionString);
});

builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Clase para mapear las opciones de Azure Blob Storage
public class AzureBlobOptions
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}