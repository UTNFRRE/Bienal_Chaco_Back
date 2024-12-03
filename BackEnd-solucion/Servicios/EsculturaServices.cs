using System;
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
using Microsoft.AspNetCore.Http;
using Requests;
using Contexts;
using Models;
using Servicios;

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

        public async Task<Esculturas>? CreateAsync(EsculturaPostPut request) //cambiar por esculturaRequest
        {
            //validación si exise otra escultora con el mismo nombre
            var esculturaExistente = this._context.Esculturas.FirstOrDefault(e => e.Nombre == request.Nombre);
            
            if (esculturaExistente != null)
            {
                return null;
            }

            var newEscultura = new Esculturas()
            {
                Nombre = request.Nombre,
                EscultoresID = request.EscultorID,
                Descripcion = request.Descripcion,
                FechaCreacion = request.FechaCreacion,
                Tematica = request.Tematica,
                EdicionAño = request.EdicionAño
            };  

            if (request.Imagen!= null) //cambiar por lo que viene en el request
            {
                newEscultura.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen);
            }

           await this._context.Esculturas.AddAsync(newEscultura);
           await this._context.SaveChangesAsync();

            return newEscultura;

        }

        //Este usa el front
        public async Task<IEnumerable<EsculturasListLiteDTO>> GetAllList( int pageNumber , int pageSize, int? AnioEdicion =null, string? busqueda = null)
        {
            if (busqueda != null) //Depende de si hay un parametro busqueda, llama al metodo GetAllFilter
            {
                return await GetAllFilterEsc(pageNumber, pageSize, AnioEdicion, busqueda);
            }
            var listescultura = await this._context.Esculturas
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
            var escultura = await this._context.Esculturas.FindAsync(id);
            
            if (escultura == null)
            {
                return null;
            }
            await this.asignarPromedio(escultura);
            var escultor = await this._context.Escultores.FindAsync(escultura.EscultoresID);
            EsculturasDetailDTO EsculturaDetalle = new EsculturasDetailDTO(escultura, escultor);
            return EsculturaDetalle;
            
        }

        ///modificar UpdateAsync para sobrecargar con parametro EsculturaPatchRequest
        public async Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPostPut request)
        {
            //validación si existe id del escultor

            /*var escultorExistente = this._context.Escultores.FirstOrDefault(e => e.EscultorId == request.EscultorID);
            if (esculturaExistente == null)
            {
                return null;
            }*/

        var esculturaToUpdate = this._context.Esculturas.Find(id);
            if (esculturaToUpdate != null)
            {
                esculturaToUpdate.Nombre = request.Nombre;
                esculturaToUpdate.EscultoresID = request.EscultorID;
                esculturaToUpdate.Descripcion = request.Descripcion;
                esculturaToUpdate.FechaCreacion = request.FechaCreacion;
                esculturaToUpdate.Tematica = request.Tematica;

                //control de errores nuevo nombre de escultura no existe
               
                if (request.Imagen != null)
                {
                    esculturaToUpdate.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen, esculturaToUpdate.Imagenes);
                }
                
                this._context.Update(esculturaToUpdate);
                await this._context.SaveChangesAsync();
            }
            
            return esculturaToUpdate;
        }

        public async Task<Esculturas> VoteEscultura(int id, EsculturaVoto request)
            {
                var escultura = await this._context.Esculturas.FindAsync(id);
                
                if (escultura == null)
                {
                    throw new Exception("Escultura no encontrada");
                }

                this._context.Update(escultura);
                await this._context.SaveChangesAsync();
                return escultura;
        }

        public async Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request)
        {
            var esculturaToUpdate = await this._context.Esculturas.FindAsync(id);

            if (esculturaToUpdate == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Nombre))
            {
                esculturaToUpdate.Nombre = request.Nombre;
            }


            if (!string.IsNullOrEmpty(request.Descripcion))
            {
                esculturaToUpdate.Descripcion = request.Descripcion;
            }

            if (request.Imagen != null)
            {
                esculturaToUpdate.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen, esculturaToUpdate.Imagenes);
            }

            if (request.EscultorID != null)
            {
                //conversión implicita de int? a int
                esculturaToUpdate.EscultoresID = (int)request.EscultorID;
            }

            if (request.FechaCreacion != null)
            {
                esculturaToUpdate.FechaCreacion = (DateOnly)request.FechaCreacion;
            }

            this._context.Update(esculturaToUpdate);

            await this._context.SaveChangesAsync();
            return esculturaToUpdate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var esculturaToDelete = await this._context.Esculturas.FindAsync(id);

            if (esculturaToDelete == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(esculturaToDelete.Imagenes))
            {
                await this._azureStorageService.DeleteAsync(esculturaToDelete.Imagenes);
            }
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

        //Prueba multi imagenes
        public async Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> files)
        {
            var uploadedFileNames = new List<string>();

            if (files == null || !files.Any())
            {
                return null;
            }

            foreach (var file in files)
            {
                if (file.Length == 0) continue;

                // Aquí usamos el servicio de Azure para cargar cada archivo.
                var blobFilename = await _azureStorageService.UploadAsync(file);
                uploadedFileNames.Add(blobFilename);
            }

            return uploadedFileNames;
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
        Task<Esculturas>? CreateAsync(EsculturaPostPut request);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllList( int pageNumber , int pageSize, int? AnioEdicion, string? busqueda);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllFilterEsc(int pageNumber, int pageSize, int? AnioEdicion, string busqueda);
        Task<EsculturasDetailDTO>? GetDetail(int idEscultura);
        Task<Esculturas>? GetByAsync(int id); 
        Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPostPut request);
        Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request);
        Task<Esculturas> VoteEscultura(int id, EsculturaVoto request);
        Task<bool> DeleteAsync(int id);

        Task<List<string>> UploadImagesAsync(IEnumerable<IFormFile> files);
    
    } 

