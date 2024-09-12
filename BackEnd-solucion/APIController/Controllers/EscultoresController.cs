using Microsoft.AspNetCore.Mvc;
using EscultorModel;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
namespace APIBienal.Controllers
{
     [ApiController]
    [Route("api/[controller]")]
    public class EscultorController : ControllerBase // Cambio de Controller a ControllerBase
    {
        private EscultorService _escultorService; // Cambio de BienalDbContext a EscultorService

        public EscultorController()
        {
            _escultorService =  new EscultorService();
        }

        // GET: api/Escultor
        [HttpGet]
        public async Task<Escultor> GetAllEscultores()
        {
            var list_escultores =  _escultorService.GetAll();
            return list_escultores;
        }

        // GET: api/Escultor/5
        [HttpGet("{id}")] // Get Escultor by Id
        public async Task<Escultor> GetEscultorById(int id)
        {
            return await _context.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
        }

        // CREATE: api/Escultor
        [HttpPost]
        public async Task<Escultor> CreateEscultor(Escultor escultor)
        {
            _context.Escultores.Add(escultor);
            await _context.SaveChangesAsync();
            return (escultor);
        }

        // Uptdate: api/Escultor/5
        [HttpPut("{id}")]
        public async Task<Escultor> UpdateEscultor(int id, Escultor escultor)
        {
            _context.Escultores.Update(escultor);
            await _context.SaveChangesAsync();
            return (escultor);
        }

        // DELETE: api/Escultor/5
        [HttpDelete("{id}")]
        public async Task<Escultor> DeleteEscultor(int id)
        {
            var escultor = await _context.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id); // 
            if (escultor == null)
            {
                return null;
            }
            _context.Escultores.Remove(escultor);
            await _context.SaveChangesAsync();
            return id;
        }
    }
}
