using Microsoft.AspNetCore.Mvc;
using EscultorModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIBienal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EscultorController : ControllerBase
    {
        private EscultorService _escultorService;

        // Constructor que inicializa el servicio de escultores
        public EscultorController() 
        {
            _escultorService = new EscultorService();  // La variable local _escultorService se inicializa con el servicio de escultores
        }

        // CREATE: api/Escultor
        // Método para crear un nuevo escultor
        [HttpPost("Create")]
        public async Task<ActionResult<Escultor>> CreateEscultor(Escultor escultor)
        {
            await _escultorService.Create(escultor);
            return CreatedAtAction(nameof(GetEscultorById), new { id = escultor.EscultorId }, escultor);
        }


        // GET: api/Escultor
        // Método para obtener todos los escultores
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Escultor>>> GetAllEscultores()
        {
            var lista_escultores = await _escultorService.GetAll();
            return Ok(lista_escultores);
        }

        // GET: api/Escultor/5
        // Método para obtener un escultor por su ID
        [HttpGet("GetBy")]
        public async Task<ActionResult<Escultor>> GetEscultorById(int id)
        {
            var escultor = await _escultorService.GetById(id);
            
            if (escultor == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el escultor
            } else {
                return Ok(escultor);
            }
            
        }

        // UPDATE: api/Escultor/5
        // Método para actualizar un escultor existente
        [HttpPut("Update")]
        public async Task<ActionResult<Escultor>> UpdateEscultor(Escultor escultor)
        {
            var updatedEscultor = await _escultorService.Update(escultor);

            if (updatedEscultor == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el escultor para actualizar
            }
            else {
                return Ok(updatedEscultor);
            }
        }

        // DELETE: api/Escultor/5
        // Método para eliminar un escultor por su ID
        [HttpDelete("{id}")]
        public async Task<ActionResult<Escultor>> DeleteEscultor(int id)
        {
            var escultor = await _escultorService.Delete(id);
            if (escultor == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el escultor para eliminar
            }
            else
            {
                return Ok(escultor);

            }
        }
    }
}