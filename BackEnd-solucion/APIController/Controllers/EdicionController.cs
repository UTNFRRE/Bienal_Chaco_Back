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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
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

        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEdicionByAño(int id)
        {
            var edicionObtenida = await this.EdicionServices.GetEdicionByAño(id);

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
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEdicion(int id, [FromForm] EdicionUpdateRequest request)
        {
            var edicionActualizada = await this.EdicionServices.UpdateEdicionAsync(id, request);
            return Ok(edicionActualizada);
        }

        // Actualizar una edicion existente
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchEdicion(int id, [FromForm] EdicionPatchEdicion request)
        {
            var edicionActualizada = await this.EdicionServices.UpdatePatchEdicionAsync(id, request);
            return Ok(edicionActualizada);
        }


        // Eliminar una edicion
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEdicion(int id)
        {
            await this.EdicionServices.DeleteEdicionAsync(id);
            return Ok();
        }
    }
}
