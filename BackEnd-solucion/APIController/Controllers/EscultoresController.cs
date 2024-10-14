using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Requests;
using Servicios;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace APIController.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EscultorController : ControllerBase
    {
        private readonly ICRUDServicesEscultores escultorService;
        public EscultorController(ICRUDServicesEscultores escultoresService)
        {
            this.escultorService = escultoresService;
        }

        // CREATE: api/Escultor
        [HttpPost]
        public async Task<IActionResult> CreateEscultor([FromForm] EscultoresListRequest request)
        {
            Escultores escultorCreado = await this.escultorService.CreateAsync(request);
            return CreatedAtAction(nameof(GetEscultorById), new { id = escultorCreado.EscultorId }, escultorCreado);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllEscultores()
        {
            var lista_escultores = await this.escultorService.GetAllAsync();
            return Ok(lista_escultores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEscultorById(int id)
        {
            var escultor = await this.escultorService.GetByAsync(id);
            if (escultor == null)
            {
                return NotFound();
            }
            return Ok(escultor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEscultor(int id,[FromForm] EscultoresListRequest request)
        {
            var updatedEscultor = await this.escultorService.UpdateAsync(id, request);
            if (updatedEscultor == null)
            {
                return NotFound();
            }
            return Ok(updatedEscultor);
        }

        // DELETE: api/Escultor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscultor(int id)
        {
            await this.escultorService.DeleteAsync(id);
            return NoContent();
        }
        /*
        // Subir una imagen de escultor
        [HttpPost("UploadImagen")]
        public async Task<IActionResult> SubirImagenEscultor([FromForm] imagenService fichero)
        {
            if (fichero.Archivo == null)
            {
                return BadRequest("Archivo no válido.");
            }

            var nombreArchivo = Guid.NewGuid().ToString() + ".jpg";
            var blobClient = _blobServiceClient.GetBlobContainerClient(_containerName).GetBlobClient(nombreArchivo);

            using (var memoryStream = new MemoryStream())
            {
                await fichero.Archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = fichero.Archivo.ContentType
                };

                // Usa la sobrecarga correcta de UploadAsync
                await blobClient.UploadAsync(
                    memoryStream,
                    new BlobHttpHeaders { ContentType = fichero.Archivo.ContentType } // Aquí no usamos 'overwrite'
                );

                return Ok(new { Ruta = blobClient.Uri.AbsoluteUri });
            }
        }

        // Obtener todas las imágenes de escultores
        [HttpGet("GetAllImagenes")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllImagenesEscultores()
        {
            var imagenes = new List<string>();
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                imagenes.Add(containerClient.GetBlobClient(blobItem.Name).Uri.AbsoluteUri);
            }

            return Ok(imagenes);
        }

        // Actualizar/Reemplazar una imagen
        [HttpPut("UpdateImagen/{nombreArchivo}")]
        public async Task<IActionResult> UpdateImagen(string nombreArchivo, [FromForm] imagenService fichero)
        {
            if (fichero.Archivo == null)
            {
                return BadRequest("El archivo es nulo");
            }

            var blobClient = _blobServiceClient.GetBlobContainerClient(_containerName).GetBlobClient(nombreArchivo);

            using (var memoryStream = new MemoryStream())
            {
                await fichero.Archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Utiliza la sobrecarga adecuada de UploadAsync
                await blobClient.UploadAsync(
                    memoryStream,
                    new BlobHttpHeaders { ContentType = fichero.Archivo.ContentType }
                );

                return Ok($"Imagen {nombreArchivo} actualizada correctamente.");
            }
        }

        // Eliminar una imagen de escultor
        [HttpDelete("DeleteImagen/{nombreArchivo}")]
        public async Task<IActionResult> EliminarImagenEscultor(string nombreArchivo)
        {
            var blobClient = _blobServiceClient.GetBlobContainerClient(_containerName).GetBlobClient(nombreArchivo);

            var response = await blobClient.DeleteIfExistsAsync();

            if (response)
            {
                return Ok($"Imagen {nombreArchivo} eliminada correctamente.");
            }
            else
            {
                return StatusCode(500, "Error al eliminar la imagen.");
            }
        }*/
    }
}
