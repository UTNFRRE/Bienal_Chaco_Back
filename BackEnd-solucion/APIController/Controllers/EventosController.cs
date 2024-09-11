using Microsoft.AspNetCore.Mvc;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventosController : ControllerBase
    {
        [HttpPost("Create")]
        public IActionResult Create()
        {
            // L?gica para crear una nueva entidad de evento
            return Ok();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            // L?gica para obtener todos los eventos
            return Ok();
        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(int id)
        {
            // L?gica para obtener un evento por su ID
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update(int id)
        {
            // L?gica para actualizar un evento
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            // L?gica para eliminar un evento
            return Ok();
        }
    }
}