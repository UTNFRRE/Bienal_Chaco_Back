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
using static Servicios.Ediciones;
using Models;

namespace APIBienal.Controllers
{

    [Route("[controller]")]
    [ApiController]

    public class EdicionController : ControllerBase
    {
        private readonly ICRUDServiceEdicion EdicionServices;

        public EdicionController(ICRUDServiceEdicion edicionServices)
        {
            this.EdicionServices = edicionServices;
        }

        // Crear una nueva edicion
        [HttpPost]
        public async Task<IActionResult> CreateEdicion([FromForm] EdicionPostRequests request)
        {
            var createdEdicion = await this.EdicionServices.CreateEdicionAsync(request);
            return Ok(createdEdicion);
        }

        // Obtener todas las ediciones
        [HttpGet]
        public async Task<IActionResult> GetAllEdiciones()
        {
            var listaDeEdiciones = await this.EdicionServices.GetAllEdicionAsync();
            return listaDeEdiciones == null ? NotFound() : Ok(listaDeEdiciones);
        }


        // Obtener una edicion por ID
        [HttpGet("{año}")]
        public async Task<IActionResult> GetEdicionByAño(int año)
        {
            var edicionObtenida = await this.EdicionServices.GetEdicionByAño(año);

            if (edicionObtenida == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(edicionObtenida);
            }

        }

        // Actualizar una edicion existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEdicion(int id, [FromForm] EdicionUpdateRequest request)
        {
            var edicionActualizada = await this.EdicionServices.UpdateEdicionAsync(id, request);
            return Ok(edicionActualizada);
        }

        // Actualizar una edicion existente
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchEdicion(int id, [FromForm] EdicionPatchEdicion request)
        {
            var edicionActualizada = await this.EdicionServices.UpdatePatchEdicionAsync(id, request);
            return Ok(edicionActualizada);
        }


        // Eliminar una edicion
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEdicion(int id)
        {
            await this.EdicionServices.DeleteEdicionAsync(id);
            return Ok();
        }
    }
}
