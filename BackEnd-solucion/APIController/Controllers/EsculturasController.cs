using Esculturas;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly EsculturasServices _esculturaService;
        public EsculturasController()
        {
            _esculturaService = new EsculturasServices();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CrearEscultura(EsculturasModel escultura)
        {
            await _esculturaService.CrearEscultura(escultura);
            return Ok();
        }

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

        [HttpPut("Update")]
        public async Task<IActionResult> ActualizarEscultura(EsculturasModel escultura)
        {
            await _esculturaService.ActualizarEscultura(escultura);
            return Ok();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            await _esculturaService.EliminarEscultura(id);
            return Ok();
        }
        
        [HttpPost("CreateImagen")]
        public async Task<string> GuardarImagen([FromForm] imagenService fichero)
        {
            var ruta = String.Empty;

            if (fichero.Archivo != null)
            {
                var nombreArchivo = Guid.NewGuid().ToString() + ".jpg"; //asigna un nombre único al archivo
                ruta = $"Imagenes/{nombreArchivo}"; //esto se debería reemplazar, no guardar en disco sino en un object storage

                using (var fileStream = new FileStream(ruta, FileMode.Create)) //crea el archivo en disco
                {
                    await fichero.Archivo.CopyToAsync(fileStream); //guarda el archivo en disco
                }
            }
            return ruta;
        }
        [HttpGet("GetAllImagenes")]
        public async Task<IEnumerable<string>> GetAllImagenes()
        {
            var imagenes = new List<string>();
            var directorioImagenes = "Imagenes"; // El directorio donde se guardan las imágenes
                // Obtener todos los archivos con extensión .jpg en el directorio
                var archivos = Directory.GetFiles(directorioImagenes, "*.jpg");

                // Agregar las rutas de los archivos a la lista
                foreach (var archivo in archivos)
                {
                    imagenes.Add(archivo);
                }
            return await Task.FromResult(imagenes);
        }

    }
}