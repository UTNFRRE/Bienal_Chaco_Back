using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using BienalModel;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly EsculturasServices _esculturaService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public EsculturasController(BlobServiceClient blobServiceClient, IOptions<AzureBlobOptions> azureBlobOptions)
        {
            _esculturaService = new EsculturasServices();
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(azureBlobOptions.Value.ContainerName);
        }

        // Crear Escultura (CRUD para esculturas)
        [HttpPost("Create")]
        public async Task<IActionResult> CrearEscultura(Escultura escultura)
        {
            await _esculturaService.CrearEscultura(escultura);
            return Ok();
        }

        // Obtener todas las esculturas
        [HttpGet("GetAll")]
        public async Task<IActionResult> ObtenerTodasLasEsculturas()
        {
            var esculturas = await _esculturaService.ObtenerTodasLasEsculturas();
            if (esculturas == null || esculturas.Count == 0)
            {
                return NotFound();
            }
            return Ok(esculturas);
        }

        // Obtener escultura por ID
        [HttpGet("GetByID")]
        public async Task<IActionResult> ObtenerEscultura(int id)
        {
            var escultura = await _esculturaService.ObtenerEscultura(id);
            if (escultura == null)
            {
                return NotFound();
            }
            return Ok(escultura);
        }

        // Actualizar escultura
        [HttpPut("Update")]
        public async Task<IActionResult> ActualizarEscultura(Escultura escultura)
        {
            await _esculturaService.ActualizarEscultura(escultura);
            return Ok();
        }

        // Eliminar escultura
        [HttpDelete("Delete")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            await _esculturaService.EliminarEscultura(id);
            return Ok();
        }

        // ====================================================
        // CRUD para Imágenes
        // ====================================================

        // Crear Imagen (subir a Blob Storage)
        [HttpPost("CreateImagen")]
        public async Task<IActionResult> GuardarImagen([FromForm] imagenService fichero)
        {
            if (fichero.Archivo == null)
            {
                return BadRequest("El archivo es nulo");
            }

            var nombreArchivo = Guid.NewGuid().ToString() + ".jpg"; // Nombre único para el archivo
            var blobClient = _blobContainerClient.GetBlobClient($"Imagenes/{nombreArchivo}");

            using (var memoryStream = new MemoryStream())
            {
                await fichero.Archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = fichero.Archivo.ContentType });
                return Ok(new { Ruta = blobClient.Uri.AbsoluteUri }); // Devuelve la URL de la imagen en Blob Storage
            }
        }

        // Obtener todas las imágenes (desde Blob Storage)
        [HttpGet("GetAllImagenes")]
        public async Task<IEnumerable<string>> GetAllImagenes()
        {
            var imagenes = new List<string>();
            await foreach (var blobItem in _blobContainerClient.GetBlobsAsync(prefix: "Imagenes/"))
            {
                imagenes.Add(_blobContainerClient.GetBlobClient(blobItem.Name).Uri.AbsoluteUri);
            }
            return imagenes;
        }

        // Obtener una imagen por nombre o ID
        [HttpGet("GetImagen/{nombreArchivo}")]
        public async Task<IActionResult> GetImagen(string nombreArchivo)
        {
            var blobClient = _blobContainerClient.GetBlobClient($"Imagenes/{nombreArchivo}");

            try
            {
                var response = await blobClient.DownloadAsync();
                return File(response.Value.Content, response.Value.ContentType);
            }
            catch (Azure.RequestFailedException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // Actualizar/Reemplazar una imagen
        [HttpPut("UpdateImagen/{nombreArchivo}")]
        public async Task<IActionResult> UpdateImagen(string nombreArchivo, [FromForm] imagenService fichero)
        {
            if (fichero.Archivo == null)
            {
                return BadRequest("El archivo es nulo");
            }

            var blobClient = _blobContainerClient.GetBlobClient($"Imagenes/{nombreArchivo}");

            using (var memoryStream = new MemoryStream())
            {
                await fichero.Archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                await blobClient.UploadAsync(
                    memoryStream,
                    new BlobHttpHeaders { ContentType = fichero.Archivo.ContentType }
                );
                return Ok($"Imagen {nombreArchivo} actualizada correctamente.");
            }
        }

        // Eliminar una imagen
        [HttpDelete("DeleteImagen/{nombreArchivo}")]
        public async Task<IActionResult> DeleteImagen(string nombreArchivo)
        {
            var blobClient = _blobContainerClient.GetBlobClient($"Imagenes/{nombreArchivo}");

            try
            {
                await blobClient.DeleteIfExistsAsync();
                return Ok($"Imagen {nombreArchivo} eliminada correctamente.");
            }
            catch (Azure.RequestFailedException ex)
            {
                return StatusCode(500, "Error al eliminar la imagen: " + ex.Message);
            }
        }
    }
}