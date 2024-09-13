using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EscultorModel
{
    public class BienalDbContext : DbContext
    {
        public DbSet<Escultor> Escultores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // InMemoryDatabase para pruebas, después cambiar a SQL Server
            optionsBuilder.UseInMemoryDatabase("BienalDB");
            // optionsBuilder.UseSqlServer(@"Server=.\;Database=BienalDB;Trusted_Connection=True;");
        }
    }

    public class Escultor
    {
        public int EscultorId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? DNI { get; set; }
        public string? Pais { get; set; }
        public string? Email { get; set; }
        public string? Contraseña { get; set; }
        public string? Telefono { get; set; }
        public string? Biografia { get; set; }
    }

    public class EscultorService
    {
        private BienalDbContext _context;

        public EscultorService() // Constructor sin parámetros de escultor service 
        {
            _context = new BienalDbContext();
        }

        public async Task<Escultor> Create(Escultor escultor) //Service method to create a new escultor
        {
            _context.Escultores.Add(escultor);
            await _context.SaveChangesAsync();
            return escultor;
        }

        public async Task<IEnumerable<Escultor>> GetAll() //Service method para obtener todos los escultores
        {
            var lista = await _context.Escultores.ToListAsync();
            return lista;
        }

        public async Task<Escultor?> GetById(int id) //Service method obtener escultor por id
        {
            return await _context.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
        }

        public async Task<Escultor> Update(Escultor escultor) //service method para actualizar un escultor
        {
            _context.Escultores.Update(escultor);
            await _context.SaveChangesAsync();
            return escultor;
        }

        public async Task<Escultor?> Delete(int id) //service method para eliminar un escultor
        {
            var escultor = await _context.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
            if (escultor != null)
            {
                _context.Escultores.Remove(escultor);
                await _context.SaveChangesAsync();
            }
            return escultor;
        }
    }
}