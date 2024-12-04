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

        public async Task<Esculturas> CreateAsync(EsculturaPostPut request)
        {
            var newEscultura = new Esculturas
            {
                Nombre = request.Nombre,
                Descripcion = request.Descripcion
            };

            // Si hay imágenes en el request, las agregamos
            if (request.Imagenes != null && request.Imagenes.Any())
            {
                foreach (var file in request.Imagenes)
                {
                    var iFormFile = ConvertToIFormFile(file);
                    var url = await _azureStorageService.UploadAsync(iFormFile); // Sube la imagen y obtiene la URL
                    newEscultura.Imagenes.Add(new Imagen
                    {
                        Url = url,
                        NombreArchivo = file.NombreArchivo
                    });
                }
            }

            await _context.Esculturas.AddAsync(newEscultura);
            await _context.SaveChangesAsync();
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
               
            if (request.Imagenes != null && request.Imagenes.Any())
            {
                // Convierte cada imagen en la lista a IFormFile
                var formFiles = request.Imagenes.Select(imagen => ConvertToIFormFile(imagen)).ToList();

                // Itera sobre cada IFormFile y llama a UploadAsync para cada uno
                foreach (var formFile in formFiles)
                {
                    string blobFileName = Path.GetFileName(formFile.FileName);
                    await this._azureStorageService.UploadAsync(formFile, blobFileName);
                }
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

            // Si la lista de imágenes no es null y contiene elementos
            if (request.Imagenes != null && request.Imagenes.Any())
            {
                // Convierte cada imagen en la lista a IFormFile
                var formFiles = request.Imagenes.Select(imagen => ConvertToIFormFile(imagen)).ToList();

                // Itera sobre cada IFormFile y llama a UploadAsync para cada uno
                foreach (var formFile in formFiles)
                {
                    // Obtén el nombre del archivo para el blob
                    string blobFileName = Path.GetFileName(formFile.FileName);

                    // Subir la imagen y obtener el nombre del archivo del blob o alguna URL
                    var uploadedFile = await this._azureStorageService.UploadAsync(formFile, blobFileName);

                    // Aquí deberías agregar el objeto de la imagen subida a la lista de imágenes de la escultura.
                    // Suponiendo que `Imagenes` es una lista de objetos `Imagen`, agregamos un nuevo objeto Imagen con el blob file name.
                    esculturaToUpdate.Imagenes.Add(new Imagen { FilePath = uploadedFile }); // Asegúrate de agregar la lógica correcta para agregar imágenes a la lista
                }
            }

            if (request.EscultorID != null)
            {
                // Conversión implícita de int? a int
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

        //Prueba multi imagenes
        // Implementación del método UploadImagesAsync
        public async Task<IList<string>> UploadImagesAsync(int esculturaId, IFormFile[] files)
        {
            var imagenesCargadas = new List<string>();

            var escultura = await _context.Esculturas
                .Include(e => e.Imagenes)  // Asegúrate de incluir las imágenes
                .FirstOrDefaultAsync(e => e.EsculturaId == esculturaId);

            if (escultura == null)
            {
                throw new Exception("Escultura no encontrada");
            }

            foreach (var file in files)
            {
                var filename = await _azureStorageService.UploadAsync(file);

                // Crear un objeto Imagen para cada archivo
                var imagen = new Imagen
                {
                    NombreArchivo = filename,
                    EsculturaId = esculturaId,
                };

                // Agregar la imagen a la colección de imágenes de la escultura
                escultura.Imagenes.Add(imagen);
                imagenesCargadas.Add(filename);
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return imagenesCargadas;
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
        Task<Esculturas>? CreateAsync(EsculturaPostPut request);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllList( int pageNumber , int pageSize, int? AnioEdicion, string? busqueda);
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllFilterEsc(int pageNumber, int pageSize, int? AnioEdicion, string busqueda);
        Task<EsculturasDetailDTO>? GetDetail(int idEscultura);
        Task<Esculturas>? GetByAsync(int id); 
        Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPostPut request);
        Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request);
        Task<Esculturas> VoteEscultura(int id, EsculturaVoto request);
        Task<bool> DeleteAsync(int id);

        Task<IList<string>> UploadImagesAsync(int esculturaId, IFormFile[] files);
        Task<List<Imagen>> GetImagenesByEsculturaAsync(int esculturaId);
    
    } 

