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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> CreateEvento([FromForm]EventoCreateRequest request)
        {
            var createdEvento = await this.eventoService.CreateEventoAsync(request);
            return Ok(createdEvento);
        }

        // Obtener todos los eventos
        [HttpGet]
        public async Task<IActionResult> GetAllEventos(int? AnioEdicion = null)
        {
            var listadeeventos = await this.eventoService.GetAllEventosAsync(AnioEdicion);
            return listadeeventos == null ? NotFound() : Ok (listadeeventos);
        }


        // Obtener un evento por ID
        [HttpGet("{id}")]
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
        
        // obtener de una fecha

        [HttpGet("fecha/{fecha}")]
        public async Task<IActionResult> GetEventosByFecha( DateTime fecha)
        {
            var listadeeventos = await this.eventoService.GetEventosByFechaAsync(fecha);
            return listadeeventos == null ? NotFound() : Ok (listadeeventos);
        }

        // obtener eventos proximos
        [HttpGet("next")]
        public async Task<IActionResult> GetEventosNext(int? AnioEdicion = null)
        {
            var listadeeventos = await this.eventoService.GetEventosNextAsync(AnioEdicion);
            return listadeeventos == null ? NotFound() : Ok (listadeeventos);

        }



        // Actualizar un evento existente
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvento(int id,[FromForm] EventoUpdateRequest request)
        {
           Eventos eventoActualizado = await this.eventoService.UpdateEventoAsync(id, request);
           return Ok(eventoActualizado);
        }

        // Actualizar un evento existente
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchEvento(int id,[FromForm] EventoPatchRequest request)
        {
           Eventos eventoActualizado = await this.eventoService.UpdatePatchEventoAsync(id, request);
           return Ok(eventoActualizado);
        }


        // Eliminar un evento
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            await this.eventoService.DeleteEventoAsync(id);
            return Ok();
        }
    }
}
    
