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

namespace APIBienal.Controllers
{
    
    [Route("[controller]")]
    [ApiController]

    public class EventosController : ControllerBase
    {
        private readonly ICRUDServiceEvent eventoService;

        public EventosController(ICRUDServiceEvent eventosServices)
        {
            this.eventoService = eventosServices;
        }

        // Crear un nuevo evento
        [HttpPost("Create")]
        public async Task<IActionResult> CreateEvento([FromForm]EventoListRequest request)
        {
            var createdEvento = await this.eventoService.CreateEventoAsync(request);
            return Ok(createdEvento);
        }

        // Obtener todos los eventos
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllEventos()
        {
            var listadeeventos = await this.eventoService.GetAllEventosAsync();
            return listadeeventos == null ? NotFound() : Ok (listadeeventos);
        }

        // Obtener un evento por ID
        [HttpGet("Getbyid")]
        public async Task<IActionResult> GetEventoById(int id)
        {
            var eventoObtenido = await this.eventoService.GetEventoByIdAsync(id);

            if (eventoObtenido == null)
            {
                return NotFound();
            }
            else {
                return Ok(eventoObtenido);
            }
            
        }
        // Actualizar un evento existente
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateEvento(int id,[FromForm] EventoListRequest request)
        {
           Eventos eventoActualizado = await this.eventoService.UpdateEventoAsync(id, request);
           return Ok(eventoActualizado);
        }

        // Eliminar un evento
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            await this.eventoService.DeleteEventoAsync(id);
            return Ok();
        }
    }
}
    
