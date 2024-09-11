using Microsoft.AspNetCore.Mvc;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EsculturasController : ControllerBase
    {
        [HttpPost("Create")]
        public IActionResult Create()
        {
            // L?gica para crear una nueva entidad de escultura
            return Ok();
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            // L?gica para obtener todas las esculturas
            return Ok();
        }

        [HttpGet("GetByID")]
        public IActionResult GetByID(int id)
        {
            // L?gica para obtener una escultura por su ID
            return Ok();
        }

        [HttpPut("Update")]
        public IActionResult Update(int id)
        {
            // L?gica para actualizar una escultura
            return Ok();
        }

        [HttpDelete("Delete")]
        public IActionResult Delete(int id)
        {
            // L?gica para eliminar una escultura
            return Ok();
        }
    }
}