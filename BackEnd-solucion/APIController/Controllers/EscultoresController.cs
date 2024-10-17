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
        private string url = "https://bienalobjectstorage.blob.core.windows.net/imagenes/";
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
            foreach (var escultor in lista_escultores)
            {
                escultor.Foto = this.url + escultor.Foto;
            }
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
            escultor.Foto = this.url + escultor.Foto;
            return Ok(escultor);
        }
        //get escultor detail
        [HttpGet("/escultoresAdmin")]
        public async Task<IActionResult> GetDetail()
        {
            var esc = await escultorService.GetEscultorDetailAsync();
            if (esc == null)
            {
                return NotFound();
            }
            return Ok(esc);
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

        //Devolver todas las esculturas de un escultor
        [HttpGet("{id}/esculturas")]
        public async Task<IActionResult> getEsculturas(int id)
        {
            var esc= await escultorService.getEsculturas(id);
            if (esc == null)
            {
                return NoContent();
            }
            foreach(var escultura in esc)
            {
                escultura.imagenes = this.url + escultura.imagenes;
            }
            return Ok(esc);
        }
        //get public escultores
        [HttpGet("api/escultoresPublic")]
        public async Task<IActionResult> getEscultoresPublic()
        {
            var esc = await escultorService.getEscultoresPublic();
            if (esc == null)
            {
                return NoContent();
            }
            return Ok(esc);
        }
    }
}
