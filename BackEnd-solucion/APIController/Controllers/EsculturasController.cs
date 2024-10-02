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

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly ICRUDService esculturaService;
    
        public EsculturasController(ICRUDService esculturasService)
        {
            this.esculturaService = esculturasService;
        }

        // Crear Escultura (CRUD para esculturas)
        [HttpPost("Create")]
        public async Task<IActionResult>CrearEscultura([FromForm] EsculturaListRequest request)
        {
            Esculturas esculturaCreate = await this.esculturaService.CreateAsync(request);
            return Ok(esculturaCreate);
        }

        // Obtener todas las esculturas
        [HttpGet("GetAll")]
        public async Task<IActionResult> ObtenerTodasLasEsculturas()
        {
            var esculturas = await this.esculturaService.GetAllAsync();
            if (esculturas == null )
            {
                return NotFound();
            }
            return Ok(esculturas);
        }

        // Obtener escultura por ID
        [HttpGet("GetByID")]
        public async Task<IActionResult> ObtenerEscultura(int id)
        {
            var escultura = await this.esculturaService.GetByAsync(id);
            if (escultura == null)
            {
                return NotFound();
            }
            return Ok(escultura);
        }

        // Actualizar escultura
        [HttpPut("Update {id}")]
        public async Task<IActionResult> ActualizarEscultura([FromRoute] int id, [FromForm] EsculturaListRequest request)
        {
            Esculturas esculturaUpdate = await this.esculturaService.UpdateAsync(id, request);
            return Ok(esculturaUpdate);
        }

        // Eliminar escultura
        [HttpDelete("Delete {id}")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            await this.esculturaService.DeleteAsync(id);
            return Ok();
        }
        
    }
}