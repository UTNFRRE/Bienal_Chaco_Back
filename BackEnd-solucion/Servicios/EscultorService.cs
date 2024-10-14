﻿using System.Collections.Generic;  // Para el uso de colecciones genéricas como IEnumerable.
using System.Threading.Tasks;      
using Azure.Storage.Blobs;         // Para el servicio de almacenamiento de blobs de Azure.
using Entidades;                   // Para el modelo de entidad 'Escultores'.
using Microsoft.EntityFrameworkCore; // Para usar Entity Framework Core, especialmente consultas y manipulación de datos.
using Requests;                
using Contexts;

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
                Pais = request.Pais,
                Email = request.Email,
                Contraseña = request.Contraseña,
                Telefono = request.Telefono,
                Biografia = request.Biografia
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
        public async Task<IEnumerable<Escultores>> GetAllAsync()
        {
            return await _context.Escultores.ToListAsync();
        }

        // Método asíncrono para obtener un escultor por su ID.
        public async Task<Escultores> GetByAsync(int id)
        {
            return await _context.Escultores.FindAsync(id);
        }

        // Método asíncrono para actualizar un escultor existente.
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
                escultorToUpdate.Email = request.Email;
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
    }
    // Interfaz genérica para las operaciones CRUD.
    public interface ICRUDServicesEscultores
    {
        Task <Escultores> CreateAsync(EscultoresListRequest request);
        Task<IEnumerable<Escultores>> GetAllAsync();
        Task<Escultores> GetByAsync(int id);
        Task<Escultores> UpdateAsync(int id, EscultoresListRequest request);
        Task DeleteAsync(int id);
    }
}