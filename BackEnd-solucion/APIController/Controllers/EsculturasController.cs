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
        private readonly TokenService _tokenService;

        public EsculturasController(ICRUDEsculturaService esculturasService)
        {
            this._tokenService = new TokenService();
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


        // Prueba multi imagenes
        [HttpPost("uploadImages")]
        public async Task<IActionResult> UploadImagesAsync([FromForm] IEnumerable<IFormFile> files)
        {
            // Llamar al servicio para cargar las imágenes
            var uploadedFileNames = await esculturaService.UploadImagesAsync(files);

            if (uploadedFileNames == null || !uploadedFileNames.Any())
            {
                return BadRequest("No files uploaded.");
            }

            return Ok(new { filenames = uploadedFileNames });
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
        // Este usa el front
        [HttpGet("GetAll")]
        public async Task<IActionResult> ObtenerListaEsculturas( int pageNumber = 1, int pageSize = 10, int? AnioEdicion = null, string? busqueda = null)
        {
            var esculturaDetail = await this.esculturaService.GetAllList(pageNumber, pageSize, AnioEdicion, busqueda);
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
        //Voto de Escultura
        [HttpPatch("{id}/Votar")]
        public async Task<IActionResult> Votacion(int id, [FromForm] EsculturaVoto request)
        {
            Esculturas? esculturaUpdate = await this.esculturaService.VoteEscultura(id, request);
            if (esculturaUpdate == null)
            {
                return NotFound("Ocurrio un error al votar la escultura. Intentelo nuevamente. Verifique si existe la escultura");
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

        // Generar token
        [HttpGet("GetToken")]
        public IActionResult GenerarToken(int esculturaId)
        {
            var token = _tokenService.GenerateToken(esculturaId);
            return Ok(new { token });
        }

        [HttpHead("Token")]
        public IActionResult ValidarToken([FromHeader] string token, [FromHeader] int idEscultura)
        {
            var isValid = _tokenService.ValidateToken(token, idEscultura);

            if (!isValid)
            {
                return Unauthorized(new { isValid = false, message = "Token inv�lido o no coincide con la escultura." });
            }

            return Ok(new { isValid = true, message = "Token v�lido." });
        }


    }
}