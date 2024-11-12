using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Entidades;
using Servicios;
using Requests;
using Microsoft.AspNetCore.Http.HttpResults;
using static System.Net.WebRequestMethods;

//ESTO ME DIJO GPT QUE LO AGREGUE PARA CORREGIR EL GETFILEPATH
using Microsoft.AspNetCore.Hosting; 
using Microsoft.Extensions.Hosting;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly ICRUDEsculturaService esculturaService;
        public EsculturasController(ICRUDEsculturaService esculturasService)
        {
            this.esculturaService = esculturasService;
        }

        //ESTO TAMBIEN AGREGO EL CHATGPT PARA CORREGIR EL GETFILEPATH
        private readonly IWebHostEnvironment _environment;
        // Inyección de IWebHostEnvironment a través del constructor
        public EsculturasController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Crear Escultura (CRUD para esculturas)
        [HttpPost]
        public async Task<IActionResult> CrearEscultura([FromForm] EsculturaPostPut request)
        {
            Esculturas esculturaCreate = await this.esculturaService.CreateAsync(request);
            if (esculturaCreate == null)
            {
                return BadRequest("Ya existe una escultura con nombre asignado");
            }
            return CreatedAtAction(nameof(ObtenerEscultura), new { id = esculturaCreate.EsculturaId }, esculturaCreate);
        }

        // Obtener escultura por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEscultura(int id)
        {
        var escultura = await this.esculturaService.GetByAsync(id);
        if (escultura == null)
        {
            return NotFound("No se encontro una escultura con el id proporcionado");
        }

        //devolver link de imagen hardcodeo url de azure
        escultura.Imagenes = "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + escultura.Imagenes;

        return Ok(escultura);
        }

        [HttpGet("GetDetail/{id}")]
        public async Task<IActionResult> ObtenerDetalleEscultura(int id)
        {
            var esculturaDetail = await this.esculturaService.GetDetail(id);
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }

            return Ok(esculturaDetail);
        }
        // Este usa el front
        [HttpGet("GetAll")]
        public async Task<IActionResult> ObtenerListaEsculturas( int pageNumber = 1, int pageSize = 10, int? AnioEdicion = null)
        {
            var esculturaDetail = await this.esculturaService.GetAllList(pageNumber, pageSize, AnioEdicion);
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro ninguna escultura");
            }

            

            return Ok(esculturaDetail);
        }

        // Actualizar escultura
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTodaEscultura(int id, [FromForm] EsculturaPostPut request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.UpdatePutEsculturaAsync(id, request);
                if (esculturaUpdate == null)
                { 
                    return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura");
                }   
             return Ok(esculturaUpdate);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPropiedadEscultura(int id, [FromForm] EsculturaPatch request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.UpdatePatchAsync(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura o si ya existe otra escultura con el nombre proporcionado");
            }
            return Ok(esculturaUpdate);
        }
        //Voto de Escultura
        [HttpPatch("{id}/Votar")]
        public async Task<IActionResult> Votacion(int id, [FromForm] EsculturaVoto request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.VoteEscultura(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al votar la escultura. Intentelo nuevamente. Verifique si existe la escultura");
            }
            return Ok(esculturaUpdate);
        }


        //implementar patch con imagen para escultura

        // Eliminar escultura
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            var resultadoDelete = await this.esculturaService.DeleteAsync(id);
            if (resultadoDelete == false)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }
            return Ok("Se elimino exitosamente la escultura");
        }

        //TODO ESTO ES SOBRE UN SISTEMA DE ARCHIVOS (EN NUESTRO CASO STORAGE DE AZURE)
        //no se que tan util sea, ya que mas abajo hice que haga todo sobre la base de datos pero por ahora hay que probar.

        // Upload de imagen y de multiimagen
        //Upload de una sola imagen en un sistema de archivos como por ejemplo el storage de Azure
        [HttpPut("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile formFile, string esculturaID)
        {
            try
            {
                string filePath = GetFilepath(esculturaID);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                string imagePath = filePath + "\\" + esculturaID + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using (FileStream stream = System.IO.File.Create(imagePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                return Ok("Imagen subida exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //Upload de varias imagenes en un sistema de archivos como por ejemplo el storage de Azure
        [HttpPut("MultiUploadImage")]
        public async Task<IActionResult> MultiUploadImage(IFormFileCollection fileCollection, string esculturaID)
        {
            int passCount = 0;
            int errorCount = 0;
            try
            {
                string filePath = GetFilepath(esculturaID);

                if (!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach (var file in fileCollection)
                {
                    string imagePath = filePath + "\\" + file.FileName;

                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    using (FileStream stream = System.IO.File.Create(imagePath))
                    {
                        await file.CopyToAsync(stream);
                        passCount++;
                    }
                }

                return Ok($"{passCount} imágenes subidas exitosamente, {errorCount} imágenes fallaron.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //Get de una sola imagen en un sistema de archivos como por ejemplo el storage de Azure
        [HttpGet("GetImage")]
        public IActionResult GetImage(string esculturaID)
        {
            string hostUrl = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            string filePath = GetFilepath(esculturaID);
            string imagePath = filePath + "\\" + esculturaID + ".png";

            if (System.IO.File.Exists(imagePath))
            {
                string imageUrl = hostUrl + "/Upload/escultura/" + esculturaID + "/" + esculturaID + ".png";
                return Ok(imageUrl);
            }
            else
            {
                return NotFound("Imagen no encontrada.");
            }
        }

        //Get de varias imagenes en un sistema de archivos como por ejemplo el storage de Azure
        [HttpGet("GetMultiImage")]
        public async Task<IActionResult> GetMultiImage(string esculturaID)
        {
            List<string> imageUrls = new List<string>();
            var containerClient = new BlobContainerClient("<connection_string>", "esculturas");

            try
            {
                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: $"{esculturaID}/"))
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    imageUrls.Add(blobClient.Uri.ToString());
                }

                if (imageUrls.Count == 0)
                    return NotFound("No se encontraron imágenes para esta escultura.");

                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //Descarga de una sola imagen en un sistema de archivos como por ejemplo el storage de Azure
        [HttpGet("download")]
        public async Task<IActionResult> Download(string esculturaID)
        {
            try
            {
                string filePath = GetFilepath(esculturaID);
                string imagePath = filePath + "\\" + esculturaID + ".png";

                if (System.IO.File.Exists(imagePath))
                {
                    MemoryStream stream = new MemoryStream();
                    using (FileStream fileStream = new FileStream(imagePath, FileMode.Open))
                    {
                        await fileStream.CopyToAsync(stream);
                    }
                    stream.Position = 0;
                    return File(stream, "image/png", esculturaID + ".png");
                }
                else
                {
                    return NotFound("Imagen no encontrada.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //Remove de una sola imagen en un sistema de archivos como por ejemplo el storage de Azure
        [HttpDelete("remove")]
        public async Task<IActionResult> Remove(string esculturaID)
        {
            try
            {
                var containerClient = new BlobContainerClient("<connection_string>", "esculturas");
                var blobClient = containerClient.GetBlobClient($"{esculturaID}.png");

                if (await blobClient.ExistsAsync())
                {
                    await blobClient.DeleteAsync();
                    return Ok("Imagen eliminada exitosamente.");
                }
                else
                {
                    return NotFound("Imagen no encontrada.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //Remove de varias imagenes en un sistema de archivos como por ejemplo el storage de Azure
        [HttpDelete("multiremove")]
        public async Task<IActionResult> MultiRemove(string esculturaID)
        {
            var containerClient = new BlobContainerClient("<connection_string>", "esculturas");
            int deleteCount = 0;

            try
            {
                await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: $"{esculturaID}/"))
                {
                    var blobClient = containerClient.GetBlobClient(blobItem.Name);
                    await blobClient.DeleteAsync();
                    deleteCount++;
                }

                return Ok($"{deleteCount} imágenes eliminadas exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        //AHORA TRABAJA SOBRE LA BASE DE DATOS

        //Todo lo que seria con la base de datos puede tener error en el nombre de la tabla o en el nombre de los datos
        //de la tabla, ya que yo (Augusto) no puedo ver la base de datos.

        //Upload de varias imagenes en la base de datos
        // Ruta para cargar múltiples imágenes
        [HttpPut("DBMultiUploadImage")]
        public async Task<IActionResult> DBMultiUploadImage(IFormFileCollection filecollection, string EsculturaID)
        {
            var (passcount, errorcount) = await esculturaService.DBMultiUploadImageAsync(filecollection, EsculturaID);

            if (errorcount > 0)
            {
                return StatusCode(500, $"Error: {errorcount} archivos fallidos.");
            }

            return Ok($"{passcount} archivos cargados correctamente.");
        }

        // Ruta para obtener todas las imágenes de una escultura
        [HttpGet("GetDBMultiImage")]
        public async Task<IActionResult> GetDBMultiImage(string EsculturaID)
        {
            var imageUrls = await esculturaService.GetDBMultiImageAsync(EsculturaID);

            if (imageUrls == null || !imageUrls.Any())
            {
                return NotFound();
            }

            return Ok(imageUrls);
        }

        // Ruta para descargar una imagen de la base de datos
        [HttpGet("dbdownload")]
        public async Task<IActionResult> dbdownload(string EsculturaID)
        {
            var image = await esculturaService.DbDownloadAsync(EsculturaID);

            if (image == null)
            {
                return NotFound();
            }

            return File(image, "image/png", $"{EsculturaID}.png");
        }

        // Crea y devuelve el PATH de una escultura
        [NonAction]
        private string GetFilepath(string EsculturaID)
        {
            return Path.Combine(_environment.WebRootPath, "Upload", "escultura", EsculturaID);
        }

    }
}