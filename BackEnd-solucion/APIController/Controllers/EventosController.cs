using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventosModel;

namespace APIController
{
    [Route("api/[controller]")]
    [ApiController]

    public class EventosController : ControllerBase
    {
        private readonly EventosServices _eventosServices;

        public EventosController(EventosServices eventosServices)
        {
            _eventosServices = eventosServices;
        }

        // Obtener todos los eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Eventos>>> GetAllEventos()
        {
            var eventos = await _eventosServices.GetAllEventosAsync();
            return Ok(eventos);
        }

        // Obtener un evento por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Eventos>> GetEventoById(int id)
        {
            var evento = await _eventosServices.GetEventoByIdAsync(id);
            if (evento == null)
            {
                return NotFound();
            }
            return Ok(evento);
        }

        // Crear un nuevo evento
        [HttpPost]
        public async Task<ActionResult<Eventos>> CreateEvento([FromBody] Eventos evento)
        {
            var createdEvento = await _eventosServices.CreateEventoAsync(evento);
            return CreatedAtAction(nameof(GetEventoById), new { id = createdEvento.Id }, createdEvento);
        }

        // Actualizar un evento existente
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvento(int id, [FromBody] Eventos evento)
        {
            if (id != evento.Id)
            {
                return BadRequest();
            }

            var result = await _eventosServices.UpdateEventoAsync(evento);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        // Eliminar un evento
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var result = await _eventosServices.DeleteEventoAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}