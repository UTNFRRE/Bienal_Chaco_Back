﻿using System.Collections.Generic;  // Para el uso de colecciones genéricas como IEnumerable.
using System.Threading.Tasks;      
using Azure.Storage.Blobs;         // Para el servicio de almacenamiento de blobs de Azure.
using Entidades;                   // Para el modelo de entidad 'Escultores'.
using Microsoft.EntityFrameworkCore; // Para usar Entity Framework Core, especialmente consultas y manipulación de datos.
using Requests;                
using Contexts;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Text.Json.Serialization;

namespace Servicios
{
    public class EscultoresServices : ICRUDServicesEscultores
    {
        // Se definen dos variables privadas para el contexto de la base de datos y el servicio de almacenamiento en Azure.
        private readonly BienalDbContext _context;
        private readonly IAzureStorageService _azureStorageService;

        public EscultoresServices(BienalDbContext context, IAzureStorageService azureStorageService)
        {
            // Asignación de los parámetros a las variables privadas de la clase.
            this._context = context;
            this._azureStorageService = azureStorageService;
        }

        // Método asíncrono para crear un nuevo escultor.
        public async Task<Escultores> CreateAsync(EscultoresListRequest request)
        {
            // Se crea una nueva instancia del modelo 'Escultores' utilizando los datos del 'request'.
            var newEscultor = new Escultores
            {
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                DNI = request.DNI,
                FechaNacimiento = request.FechaNacimiento,
                LugarNacimiento = request.LugarNacimiento,
                Premios = request.Premios,
                Pais = request.Pais,
                Telefono = request.Telefono,
                Biografia = request.Biografia,
                EdicionAño = request.EdicionAño
            };

            // Si el 'request' incluye una foto, se sube al almacenamiento en Azure y se guarda la URL en el campo 'Imagen'.
            if (request.Imagen != null)
            {
                newEscultor.Foto = await _azureStorageService.UploadAsync(request.Imagen);
            } else { newEscultor.Foto = null; }

            // Se añade el nuevo escultor a la base de datos a través del contexto de Entity Framework.
            _context.Escultores.Add(newEscultor);
            await _context.SaveChangesAsync();

            return newEscultor;
        }

        // Método asíncrono para obtener todos los escultores de la base de datos.
        public async Task<IEnumerable<Escultores>> GetAllAsync(int pageNumber , int pageSize, int? AnioEdicion = null, string? busqueda = null)
        {
            if (busqueda != null) //Depende de si hay un parametro busqueda, llama al metodo GetAllFilter
            {
                return await GetAllFilter(pageNumber, pageSize, AnioEdicion, busqueda);
            }

            return await _context.Escultores
                .Where(e => e.EdicionAño == AnioEdicion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        // Metodo asinc de filtrado con opcion a filtrado por campos de la tabla escultores. 
        public async Task<IEnumerable<Escultores>> GetAllFilter(int pageNumber, int pageSize, int? AnioEdicion, string busqueda)
        {
            return await _context.Escultores.Where(u => (u.EdicionAño == AnioEdicion) && ((u.Nombre + " " + u.Apellido).Contains(busqueda) || u.DNI.Contains(busqueda) || u.Pais.Contains(busqueda)))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // Método asíncrono para obtener un escultor por su ID.
        public async Task<Escultores> GetByAsync(int id)
        {
            return await _context.Escultores.FindAsync(id);
        }

        // Método asíncrono para actualizar un escultor existente. Usamos patch igual
        public async Task<Escultores> UpdateAsync(int id, EscultoresListRequest request)
        {
            // Se busca el escultor en la base de datos por su ID.
            var escultorToUpdate = await _context.Escultores.FindAsync(id);

            // Si se encuentra el escultor, se actualizan sus propiedades con los datos del request.
            if (escultorToUpdate != null)
            {
                escultorToUpdate.Nombre = request.Nombre;
                escultorToUpdate.Apellido = request.Apellido;
                escultorToUpdate.DNI = request.DNI;
                escultorToUpdate.Pais = request.Pais;
                escultorToUpdate.Telefono = request.Telefono;
                escultorToUpdate.Biografia = request.Biografia;

                // Si hay una nueva foto, se sube a Azure y se actualiza la URL de la foto del escultor.
                if (request.Imagen != null)
                {
                    escultorToUpdate.Foto = await _azureStorageService.UploadAsync(request.Imagen, escultorToUpdate.Foto);
                }

                // Se actualiza el registro del escultor en la base de datos.
                _context.Escultores.Update(escultorToUpdate);
                await _context.SaveChangesAsync();
            }
            return escultorToUpdate;
        }
        public async Task<Escultores> UpdatePatchAsync(int id, EscultoresPatchRequest request)
        {
            // Se busca el escultor en la base de datos por su ID.
            var escultorToUpdate = await _context.Escultores.FindAsync(id);

            // Si se encuentra el escultor, se actualizan sus propiedades con los datos del request.
            if (escultorToUpdate != null)
            {
                escultorToUpdate.Nombre = request.Nombre ?? escultorToUpdate.Nombre;
                escultorToUpdate.Apellido = request.Apellido ?? escultorToUpdate.Apellido;
                escultorToUpdate.DNI = request.DNI ?? escultorToUpdate.DNI;
                escultorToUpdate.LugarNacimiento = request.LugarNacimiento ?? escultorToUpdate.LugarNacimiento;
                escultorToUpdate.Premios = request.Premios ?? escultorToUpdate.Premios;
                escultorToUpdate.FechaNacimiento = request.FechaNacimiento;
                escultorToUpdate.Pais = request.Pais ?? escultorToUpdate.Pais;
                escultorToUpdate.Telefono = request.Telefono ?? escultorToUpdate.Telefono;
                escultorToUpdate.Biografia = request.Biografia ?? escultorToUpdate.Biografia;

                // Si hay una nueva foto, se sube a Azure y se actualiza la URL de la foto del escultor.
                if (request.Imagen != null)
                {
                    escultorToUpdate.Foto = await _azureStorageService.UploadAsync(request.Imagen, escultorToUpdate.Foto);
                }

                // Se actualiza el registro del escultor en la base de datos.
                _context.Escultores.Update(escultorToUpdate);
                await _context.SaveChangesAsync();
            }
            return escultorToUpdate;
        }
        // Método asíncrono para eliminar un escultor por su ID.
        public async Task DeleteAsync(int id)
        {
            var escultorToDelete = await _context.Escultores.FindAsync(id);

            // Si se encuentra el escultor, se procede a eliminarlo.
            if (escultorToDelete != null)
            {
                // Si el escultor tiene una foto almacenada, se elimina de Azure.
                if (!string.IsNullOrEmpty(escultorToDelete.Foto))
                {
                    await _azureStorageService.DeleteAsync(escultorToDelete.Foto);
                }

                // Se elimina el escultor de la base de datos.
                _context.Escultores.Remove(escultorToDelete);
                await _context.SaveChangesAsync();
            }
        }

        //get esculturas por id de escultor
        public async Task<IEnumerable<EsculturasEscultorDTO>> getEsculturas(int id)
        {
            var esc = await _context.Esculturas
                .Where(e => e.EscultoresID == id)  // Filtramos por escultor ID
                .Select(e => new EsculturasEscultorDTO
                {
                    id = e.EsculturaId,
                    escultorid = e.EscultoresID,
                    nombre = e.Nombre,
                    descripcion = e.Descripcion,
                    // Concatenamos las URLs de las imágenes en un solo string
                    imagenes = string.Join(", ", e.Imagenes.Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo))
                })
                .ToListAsync();
            return esc;
        }

        //get public
        public async Task<IEnumerable<object>> getEscultoresPublic(int? AnioEdicion = null) //NO tocar
        {
            var escultores = await _context.Escultores.Where(u => u.EdicionAño == AnioEdicion).Select(e => new
            {
                id = e.EscultorId,
                nombre = e.Nombre,
                pais = e.Pais,
                foto = "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + e.Foto
            }).ToListAsync();
            return escultores;
        }
        
    }
    // Interfaz genérica para las operaciones CRUD.
    public interface ICRUDServicesEscultores
    {
        Task <Escultores> CreateAsync(EscultoresListRequest request);
        Task<IEnumerable<Escultores>> GetAllAsync( int pageNumber, int pageSize, int? AnioEdicion, string? busqueda);
        Task<Escultores> GetByAsync(int id);
        Task<Escultores> UpdateAsync(int id, EscultoresListRequest request);
        Task<Escultores>? UpdatePatchAsync(int id, EscultoresPatchRequest request);
        Task DeleteAsync(int id);

        Task<IEnumerable<EsculturasEscultorDTO>> getEsculturas(int id);
        Task<IEnumerable<object>> getEscultoresPublic(int? AnioEdicion);
        Task<IEnumerable<Escultores>> GetAllFilter(int pageNumber, int pageSize, int? AnioEdicion, string busqueda);
    }

    //dto para esculturas de un escultor
    public class EsculturasEscultorDTO
    {
        public int id { get; set; }
        public string? nombre { get; set; }
        public string? descripcion { get; set; }
        public string? imagenes { get; set; }
        [JsonIgnore]
        public int escultorid { get; set; }

    }

    public class EscultorDetailDTO
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string? fechaNacimiento { get; set; }
        public string? lugarNacimiento { get; set; }
        public List<string>? premios { get; set; }
        public string foto { get; set; }
        public string? pais { get; set; }
        public string? contacto { get; set; }
    }
}
