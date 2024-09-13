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

        // GET: api/Escultor
        // Método para obtener todos los escultores
        [HttpGet]
        public async Task<IEnumerable<Escultor>> GetAllEscultores()
        {
            var lista_escultores = await _escultorService.GetAll();
            return lista_escultores;
        }

        // GET: api/Escultor/5
        // Método para obtener un escultor por su ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Escultor>> GetEscultorById(int id)
        {
            var escultor = await _escultorService.GetById(id);
            if (escultor == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el escultor
            }
            return escultor;
        }

        // CREATE: api/Escultor
        // Método para crear un nuevo escultor
        [HttpPost]
        public async Task<ActionResult<Escultor>> CreateEscultor(Escultor escultor)
        {
            await _escultorService.Create(escultor);
            return CreatedAtAction(nameof(GetEscultorById), new { id = escultor.EscultorId }, escultor);
        }

        // UPDATE: api/Escultor/5
        // Método para actualizar un escultor existente
        [HttpPut("{id}")]
        public async Task<ActionResult<Escultor>> UpdateEscultor(int id, Escultor escultor)
        {
            if (id != escultor.EscultorId)
            {
                return BadRequest(); // Retorna 400 si el ID en la URL no coincide con el ID del escultor
            }

            var updatedEscultor = await _escultorService.Update(escultor);
            if (updatedEscultor == null)
            {
                return NotFound(); // Retorna 404 si no se encuentra el escultor para actualizar
            }

            return updatedEscultor;
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

            return escultor;
        }
    }
}