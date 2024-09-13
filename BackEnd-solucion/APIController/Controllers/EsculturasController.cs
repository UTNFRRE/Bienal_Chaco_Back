using Esculturas;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
    }
}