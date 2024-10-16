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
using static System.Net.WebRequestMethods;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        private readonly ICRUDEsculturaService esculturaService;

        public EsculturasController(ICRUDEsculturaService esculturasService)
        {
            this.esculturaService = esculturasService;
        }

        // Crear Escultura (CRUD para esculturas)
        [HttpPost]
        public async Task<IActionResult> CrearEscultura([FromForm] EsculturaPostPut request)
        {
            Esculturas esculturaCreate = await this.esculturaService.CreateAsync(request);
            if (esculturaCreate == null)
            {
                return BadRequest("Ya existe una escultura con nombre asignado");
            }
            return CreatedAtAction(nameof(ObtenerEscultura), new { id = esculturaCreate.EsculturaId }, esculturaCreate);
        }

        // Obtener todas las esculturas
        [HttpGet]
        public async Task<IActionResult> ObtenerTodasLasEsculturas()
        {
            var esculturas = await this.esculturaService.GetAllAsync();
            //agregar link de imagen hardcodeo url de azure a cada imagen
            foreach (var escultura in esculturas)
            {
                escultura.Imagenes = "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + escultura.Imagenes;
            }

            if (esculturas == null)
            {
                return NotFound("No se encontraron esculturas.");
            }
            return Ok(esculturas);
        }

        // Obtener escultura por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerEscultura(int id)
    {
        var escultura = await this.esculturaService.GetByAsync(id);
        if (escultura == null)
        {
            return NotFound("No se encontro una escultura con el id proporcionado");
        }

            //devolver link de imagen hardcodeo url de azure
        escultura.Imagenes = "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + escultura.Imagenes;

        return Ok(escultura);
    }

        [HttpGet("GetDetail/{id}")]
        public async Task<IActionResult> ObtenerDetalleEscultura(int id)
        {
            var esculturaDetail = await this.esculturaService.GetDetail(id);
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }

            return Ok(esculturaDetail);
        }

        [HttpGet("GetAllLite")]
        public async Task<IActionResult> ObtenerListaEsculturas()
        {
            var esculturaDetail = await this.esculturaService.GetAllList();
            if (esculturaDetail == null)
            {
                return NotFound("No se encontro ninguna escultura");
            }

            return Ok(esculturaDetail);
        }

        // Actualizar escultura
        [HttpPut("{id}")]
    public async Task<IActionResult> ActualizarTodaEscultura(int id, [FromForm] EsculturaPostPut request)
    {
        Esculturas? esculturaUpdate = await this.esculturaService.UpdatePutEsculturaAsync(id, request);
            if (esculturaUpdate == null)
            { 
                return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura");
            }   
         return Ok(esculturaUpdate);
    }

    [HttpPatch("{id}")]
        public async Task<IActionResult> ActualizarPropiedadEscultura(int id, [FromForm] EsculturaPatch request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.UpdatePatchAsync(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al actualizar escultura. Intentelo nuevamente. Verifique si existe la escultura o si ya existe otra escultura con el nombre proporcionado");
            }
            return Ok(esculturaUpdate);
        }

        //implementar patch con imagen para escultura

        // Eliminar escultura
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarEscultura(int id)
        {
            var resultadoDelete = await this.esculturaService.DeleteAsync(id);
            if (resultadoDelete == false)
            {
                return NotFound("No se encontro una escultura con el id proporcionado");
            }
            return Ok("Se elimino exitosamente la escultura");
        }
        
    }
}