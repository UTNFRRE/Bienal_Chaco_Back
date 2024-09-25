using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BienalModel;

namespace APIController
{
    [Route("[controller]")]
    [ApiController]

    public class EventosController : ControllerBase
    {
        private readonly EventosServices _eventosServices;

        public EventosController()
        {
            _eventosServices = new EventosServices();
        }

        // Crear un nuevo evento
        [HttpPost("Create")]
        public async Task<ActionResult<Eventos>> CreateEvento(Eventos evento)
        {
            var createdEvento = await _eventosServices.CreateEventoAsync(evento);
            return Ok(createdEvento);
        }

        // Obtener todos los eventos
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Eventos>>> GetAllEventos()
        {
            var listadeeventos = await _eventosServices.GetAllEventosAsync();
            return listadeeventos == null ? NotFound() : Ok (listadeeventos);
        }

        // Obtener un evento por ID
        [HttpGet("Getbyid")]
        public async Task<ActionResult<Eventos>> GetEventoById(int id)
        {
            var eventoObtenido = await _eventosServices.GetEventoByIdAsync(id);

            if (eventoObtenido == null)
            {
                return NotFound();
            }
            else {
                return Ok(eventoObtenido);
            }
            
        }
        // Actualizar un evento existente
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateEvento(Eventos eventoActualizado)
        {
            var eventoActualizar = await _eventosServices.GetEventoByIdAsync(eventoActualizado.Id);
            eventoActualizar = eventoActualizado;

            if (eventoActualizar == null)
            {
                return NotFound();
            }

            eventoActualizar = await _eventosServices.UpdateEventoAsync(eventoActualizar);
            return Ok(eventoActualizar);
        }

        // Eliminar un evento
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var eventoaEliminar = await _eventosServices.DeleteEventoAsync(id);
            return Ok(eventoaEliminar);
        }
    }
}