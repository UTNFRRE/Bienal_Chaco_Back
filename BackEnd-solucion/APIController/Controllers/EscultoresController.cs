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

        //patch
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEscultor(int id, [FromForm] EscultoresPatchRequest request)
        {
            var patchedEscultor = await this.escultorService.UpdatePatchAsync(id, request);
            if (patchedEscultor == null)
            {
                return NotFound();
            }
            return Ok(patchedEscultor);
        }

        // DELETE: api/Escultor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEscultor(int id)
        {
            await this.escultorService.DeleteAsync(id);
            return NoContent();
        }
        
    }
}
