﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Requests;
using Contexts;
using Models;
using Servicios;

//prueba imagenes
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using System.IO;

namespace Servicios
{
    public class EsculturasServices : ICRUDEsculturaService
    {
        private BienalDbContext _context;
        private IAzureStorageService _azureStorageService;
        
        public EsculturasServices(BienalDbContext context, IAzureStorageService azureStorageService)
        {
            this._context = context;
            this._azureStorageService = azureStorageService;
        }

        //PRUEBA IMAGENES
        public IFormFile ConvertToIFormFile(Imagen imagen)
        {
            // Asegúrate de que FilePath tenga una ruta válida al archivo
            if (string.IsNullOrEmpty(imagen.FilePath) || !File.Exists(imagen.FilePath))
            {
                throw new FileNotFoundException("El archivo especificado no existe.", imagen.FilePath);
            }

            var fileStream = new FileStream(imagen.FilePath, FileMode.Open, FileAccess.Read);
            var formFile = new CustomFormFile(fileStream, Path.GetFileName(imagen.FilePath));

            return formFile;
        }

        public async Task<Esculturas> CreateAsync(EsculturaPostRequest request)
        {
            // Crear la nueva escultura
            var nuevaEscultura = new Esculturas
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion,
                FechaCreacion = request.FechaCreacion,
                EscultoresID = request.EscultorID,
                Tematica = request.Tematica,
                EdicionAño = request.EdicionAño,
                Imagenes = new List<Imagen>()
            };

            // Agregar la escultura al contexto
            await _context.Esculturas.AddAsync(nuevaEscultura);
            await _context.SaveChangesAsync(); // Guardar para obtener el ID de la escultura

            // Procesar las imágenes
            if (request.Imagenes != null && request.Imagenes.Length > 0)
            {
                foreach (var file in request.Imagenes)
                {
                    var url = await _azureStorageService.UploadAsync(file); // Subir imagen y obtener URL

                    // Crear un objeto Imagen
                    var imagen = new Imagen
                    {
                        NombreArchivo = file.FileName,
                        Url = url,
                        EsculturaId = nuevaEscultura.EsculturaId, // Asociar a la escultura recién creada
                    };

                    if (nuevaEscultura.Imagenes == null)
                    {
                        nuevaEscultura.Imagenes = new List<Imagen>();
                    }
                    nuevaEscultura.Imagenes.Add(imagen);
                }

                // Guardar los cambios en la base de datos
                await _context.SaveChangesAsync();
            }
            else
            {
                // Si no hay imágenes, asegúrate de no insertar NULL en el campo 'Imagenes'
                nuevaEscultura.Imagenes = new List<Imagen>(); // List vacía
            }

            return nuevaEscultura;
        }

        //Este usa el front
        public async Task<IEnumerable<EsculturasListLiteDTO>> GetAllList( int pageNumber , int pageSize, int? AnioEdicion =null, string? busqueda = null)
        {
            if (busqueda != null) //Depende de si hay un parametro busqueda, llama al metodo GetAllFilter
            {
                return await GetAllFilterEsc(pageNumber, pageSize, AnioEdicion, busqueda);
            }
            var listescultura = await this._context.Esculturas
                .Include(e => e.Imagenes)
                .Where(e => e.EdicionAño == AnioEdicion)
                .Skip((pageNumber -1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            await this.asignarPromedios(listescultura);

            var listesculturaDTO = new List<EsculturasListLiteDTO>();

            foreach (Esculturas esculturas in listescultura)
            {
                var escultor = await this._context.Escultores.FindAsync(esculturas.EscultoresID);
                listesculturaDTO.Add(new EsculturasListLiteDTO(esculturas, escultor));

            }
            ;
            return listesculturaDTO;
        }
        
        public async Task<IEnumerable<EsculturasListLiteDTO>> GetAllFilterEsc(int pageNumber, int pageSize, int? AnioEdicion, string busqueda)
        {
            var esculturasFiltradas = await _context.Esculturas
                .Where(u => u.EdicionAño == AnioEdicion && (u.Nombre.Contains(busqueda) || u.Tematica.Contains(busqueda)))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // asignamos prom a las esculturas filtradas
            await this.asignarPromedios(esculturasFiltradas);

            var listesculturaDTO = new List<EsculturasListLiteDTO>();

            foreach (var escultura in esculturasFiltradas)
            {
                var escultor = await this._context.Escultores.FindAsync(escultura.EscultoresID);
                listesculturaDTO.Add(new EsculturasListLiteDTO(escultura, escultor));
            }

            return listesculturaDTO;
        }


        public async Task<Esculturas>? GetByAsync(int id) //Get by id
        {
            var escultura = await this._context.Esculturas.FindAsync(id);
            await this.asignarPromedio(escultura);
            return escultura;
        }

        public async Task<EsculturasDetailDTO>? GetDetail(int id)
        {
            var escultura = await this._context.Esculturas
                .Include(e => e.Imagenes) // Incluir imágenes asociadas
                .FirstOrDefaultAsync(e => e.EsculturaId == id);
            
            if (escultura == null)
            {
                return null;
            }

            await this.asignarPromedio(escultura);

            var escultor = await this._context.Escultores.FindAsync(escultura.EscultoresID);

            EsculturasDetailDTO esculturaDetalle = new EsculturasDetailDTO(escultura, escultor);

            return esculturaDetalle;
        }

        ///modificar UpdateAsync para sobrecargar con parametro EsculturaPatchRequest
        public async Task<Esculturas?> UpdatePutEsculturaAsync(int id, EsculturaPutRequest request)
        {
            // Validar que al menos una de las dos operaciones esté presente
            if ((request.ImagenesAEliminar == null || request.ImagenesAEliminar.Length == 0) &&
                (request.NuevasImagenes == null || request.NuevasImagenes.Length == 0))
            {
                throw new Exception("Debe proporcionar imágenes para eliminar o nuevas imágenes para agregar.");
            }

            // Buscar la escultura a actualizar
            var esculturaToUpdate = await _context.Esculturas
                .Include(e => e.Imagenes) // Incluir las imágenes asociadas
                .FirstOrDefaultAsync(e => e.EsculturaId == id);

            if (esculturaToUpdate == null)
            {
                throw new Exception("Escultura no encontrada");
            }

            // Actualizar los datos principales de la escultura
            esculturaToUpdate.Nombre = request.Nombre;
            esculturaToUpdate.Descripcion = request.Descripcion;
            esculturaToUpdate.FechaCreacion = request.FechaCreacion;
            esculturaToUpdate.EscultoresID = request.EscultorID;
            esculturaToUpdate.Tematica = request.Tematica;

            // Manejar la eliminación de imágenes
            if (request.ImagenesAEliminar != null && request.ImagenesAEliminar.Length > 0)
            {
                var imagenesAEliminar = esculturaToUpdate.Imagenes
                    .Where(i => request.ImagenesAEliminar.Contains(i.Id))
                    .ToList();

                foreach (var imagen in imagenesAEliminar)
                {
                    // Eliminar la imagen del almacenamiento (Azure Blob)
                    await _azureStorageService.DeleteAsync(imagen.Url);

                    // Eliminar la imagen de la base de datos
                    _context.Imagenes.Remove(imagen);
                }
            }

            // Manejar la subida de nuevas imágenes
            if (request.NuevasImagenes != null && request.NuevasImagenes.Length > 0)
            {
                foreach (var file in request.NuevasImagenes)
                {
                    // Subir la imagen y obtener la URL
                    var url = await _azureStorageService.UploadAsync(file);

                    // Crear la nueva imagen y asociarla a la escultura
                    var nuevaImagen = new Imagen
                    {
                        NombreArchivo = file.FileName,
                        Url =  url,
                        EsculturaId = id
                    };

                    esculturaToUpdate.Imagenes.Add(nuevaImagen);
                }
            }

            // Guardar los cambios en la base de datos
            _context.Update(esculturaToUpdate);
            await _context.SaveChangesAsync();

            // Formatear las URLs de las imágenes
            esculturaToUpdate.ImagenesUrls = esculturaToUpdate.Imagenes
                .Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo)
                .ToList();

            return esculturaToUpdate;
        }

        public async Task<Esculturas?> VoteEscultura(int id, EsculturaVoto request)
        {
            // Cargar la escultura e incluir las imágenes relacionadas
            var escultura = await this._context.Esculturas
                .Include(e => e.Imagenes)
                .FirstOrDefaultAsync(e => e.EsculturaId == id);

            if (escultura == null)
            {
                throw new Exception("Escultura no encontrada");
            }

            // Actualizar las propiedades de votación
            escultura.CantVotaciones += 1;
            escultura.PromedioVotos = ((escultura.PromedioVotos * (escultura.CantVotaciones - 1)) + request.Voto) / escultura.CantVotaciones;

            // Guardar los cambios en la base de datos
            this._context.Update(escultura);
            await this._context.SaveChangesAsync();

            // Formatear las URLs de las imágenes
            escultura.ImagenesUrls = escultura.Imagenes
                .Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo)
                .ToList();

            return escultura;
        }

        public async Task<Esculturas?> UpdatePatchAsync(int id, EsculturaPatch request)
        {
            // Buscar la escultura por ID
            var esculturaToUpdate = await this._context.Esculturas
                .Include(e => e.Imagenes) // Incluir las imágenes relacionadas
                .FirstOrDefaultAsync(e => e.EsculturaId == id);

            if (esculturaToUpdate == null)
            {
                return null;
            }

            // Actualizar el nombre si está presente
            if (!string.IsNullOrEmpty(request.Nombre))
            {
                esculturaToUpdate.Nombre = request.Nombre;
            }

            // Actualizar la descripción si está presente
            if (!string.IsNullOrEmpty(request.Descripcion))
            {
                esculturaToUpdate.Descripcion = request.Descripcion;
            }

            // Manejar la eliminación de imágenes (si se proporcionan IDs para eliminar)
            if (request.ImagenesAEliminar != null && request.ImagenesAEliminar.Length > 0)
            {
                var imagenesAEliminar = esculturaToUpdate.Imagenes
                    .Where(i => request.ImagenesAEliminar.Contains(i.Id))
                    .ToList();

                foreach (var imagen in imagenesAEliminar)
                {
                    // Eliminar la imagen del almacenamiento (Azure Blob)
                    await _azureStorageService.DeleteAsync(imagen.Url);

                    // Eliminar la imagen de la base de datos
                    _context.Imagenes.Remove(imagen);
                }
            }

            // Manejar la subida de nuevas imágenes
            if (request.NuevasImagenes != null && request.NuevasImagenes.Length > 0)
            {
                foreach (var file in request.NuevasImagenes)
                {
                    var url = await _azureStorageService.UploadAsync(file); // Subir la imagen y obtener la URL

                    var nuevaImagen = new Imagen
                    {
                        NombreArchivo = file.FileName,
                        Url = url,
                        EsculturaId = id
                    };

                    esculturaToUpdate.Imagenes.Add(nuevaImagen); // Asociar la nueva imagen
                }
            }

            // Actualizar el EscultorID si está presente
            if (request.EscultorID.HasValue)
            {
                esculturaToUpdate.EscultoresID = request.EscultorID.Value;
            }

            // Actualizar la FechaCreacion si está presente
            if (request.FechaCreacion.HasValue)
            {
                esculturaToUpdate.FechaCreacion = request.FechaCreacion.Value;
            }

            // Guardar los cambios en la base de datos
            this._context.Esculturas.Update(esculturaToUpdate);
            await this._context.SaveChangesAsync();

            // Formatear las URLs de las imágenes
            esculturaToUpdate.ImagenesUrls = esculturaToUpdate.Imagenes
                .Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo)
                .ToList();

            return esculturaToUpdate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var esculturaToDelete = await this._context.Esculturas.FindAsync(id);

            if (esculturaToDelete == null)
            {
                return false;
            }

            // Verifica si hay imágenes asociadas antes de intentar eliminarlas
            if (esculturaToDelete.Imagenes != null && esculturaToDelete.Imagenes.Any())
            {
                foreach (var imagen in esculturaToDelete.Imagenes)
                {
                    // Asegúrate de pasar solo el nombre del archivo (blob filename) y no el objeto Imagen completo
                    await this._azureStorageService.DeleteAsync(imagen.NombreArchivo);
                }
            }

            // Elimina la escultura del contexto
            this._context.Esculturas.Remove(esculturaToDelete);
            await this._context.SaveChangesAsync();

            return true;
        }
        //Metodo asincrono que permite obtener los promedios de votos de cada escultura
        //Devuelve una lista de objetos EsculturaPromedio (clase creada para almacenar el id de la escultura y su promedio de votos)
        //Se agrupa por el id de la escultura y se calcula el promedio de votos de cada una. Esto con una consulta linq
        private async Task<List<EsculturaPromedio>> ObtenerPromediosAsync()
        {
            var promedios = await _context.Votos
                .GroupBy(v => v.EsculturaId)
                .Select(g => new EsculturaPromedio
                {
                    EsculturaId = g.Key,
                    Promedio = g.Average(v => v.Puntuacion)
                })
                .ToListAsync();

            return promedios;
        }
        //Metodo asincrono que asigna los promedios de votos a cada escultura
        //Recibe una lista de esculturas
        //Obtiene los promedios de votos de cada escultura y los asigna a cada escultura
        //Por default el promedio de votos de cada escultura es 0
        private async Task<bool> asignarPromedios(IEnumerable<Esculturas> esculturas)
        {
            List<EsculturaPromedio> promedios = await ObtenerPromediosAsync();
            foreach (EsculturaPromedio promedio in promedios)
            {
                var escultura = esculturas.FirstOrDefault(e => e.EsculturaId == promedio.EsculturaId);
                if (escultura != null)
                {
                    escultura.PromedioVotos = promedio.Promedio;
                }
            }
            return true;
        }
        //metodo asincrono que asigna el promedio de votos a una escultura
        //Recibe una escultura
        //Usar cuando se haga un get by id
        private async Task<bool> asignarPromedio(Esculturas escultura)
        {
            List<EsculturaPromedio> promedios = await ObtenerPromediosAsync();
            var promedio = promedios.Find(p => p.EsculturaId == escultura.EsculturaId);
            if (promedio != null)
            {
                escultura.PromedioVotos = promedio.Promedio;
            }
            return true;
        }

        public async Task<List<Imagen>> GetImagenesByEsculturaAsync(int esculturaId)
        {
            return await _context.Imagenes
                .Where(i => i.EsculturaId == esculturaId)
                .ToListAsync();
        }

    }
    public class EsculturaPromedio
    {
        public int EsculturaId { get; set; }
        public double Promedio { get; set; }
    }

}

    public interface ICRUDEsculturaService
    { 
        Task<Esculturas>? CreateAsync(EsculturaPostRequest request);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllList( int pageNumber , int pageSize, int? AnioEdicion, string? busqueda);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllFilterEsc(int pageNumber, int pageSize, int? AnioEdicion, string busqueda);
        Task<EsculturasDetailDTO>? GetDetail(int idEscultura);
        Task<Esculturas>? GetByAsync(int id); 
        Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPutRequest request);
        Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request);
        Task<Esculturas> VoteEscultura(int id, EsculturaVoto request);
        Task<bool> DeleteAsync(int id);
        Task<List<Imagen>> GetImagenesByEsculturaAsync(int esculturaId);
    
    } 

