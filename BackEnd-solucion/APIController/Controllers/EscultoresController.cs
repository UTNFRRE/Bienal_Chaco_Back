using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EscultorModel;
using Esculturas;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EscultorController : ControllerBase
    {
        private readonly EscultorService _escultorService;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public EscultorController(BlobServiceClient blobServiceClient, IConfiguration configuration)
        {
            _escultorService = new EscultorService();
            _blobServiceClient = blobServiceClient;
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName")
                            ?? throw new ArgumentNullException("AzureBlobStorage:ContainerName configuration is missing.");

            // Ensure the container exists
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            containerClient.CreateIfNotExists();
        }

        // CREATE: api/Escultor
        [HttpPost("Create")]
        public async Task<ActionResult<Escultor>> CreateEscultor(Escultor escultor)
        {
            await _escultorService.Create(escultor);
            return CreatedAtAction(nameof(GetEscultorById), new { id = escultor.EscultorId }, escultor);
        }

        // GET: api/Escultor
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Escultor>>> GetAllEscultores()
        {
            var lista_escultores = await _escultorService.GetAll();
            return Ok(lista_escultores);
        }

        // GET: api/Escultor/5
        [HttpGet("GetBy/{id}")]
        public async Task<ActionResult<Escultor>> GetEscultorById(int id)
        {
            var escultor = await _escultorService.GetById(id);
            if (escultor == null)
            {
                return NotFound();
            }
            return Ok(escultor);
        }

        // UPDATE: api/Escultor/5
        [HttpPut("Update")]
        public async Task<ActionResult<Escultor>> UpdateEscultor(Escultor escultor)
        {
            var updatedEscultor = await _escultorService.Update(escultor);
            if (updatedEscultor == null)
            {
                return NotFound();
            }
            return Ok(updatedEscultor);
        }

        // DELETE: api/Escultor/5
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<Escultor>> DeleteEscultor(int id)
        {
            var escultor = await _escultorService.Delete(id);
            if (escultor == null)
            {
                return NotFound();
            }
            return Ok(escultor);
        }

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
        }
    }
}