using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace EscultorModel
{
    public class BienalDbContext : DbContext
    {
        public DbSet<Escultor> Escultores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // InMemoryDatabase para pruebas, despues cambiar a SQL Server
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
        public string? Email { get; set; }
        public string? Contraseña { get; set; }
        public string? Telefono { get; set; }
        public string? Biografia { get; set; }
    }


    public class EscultorService
    {
        private BienalDbContext _context;

        public EscultorService() // Constructor sin parametros de escultor service 
        {
            _context = new BienalDbContext();
        }

        public Escultor Create(Escultor escultor)
        {
            _context.Escultores.Add(escultor);
            _context.SaveChanges();
            return escultor;
        }

        public IEnumerable<Escultor> GetAll()
        {
            return _context.Escultores.ToList();
        }

        public Escultor? GetById(int id)
        {
            return _context.Escultores.Single(e => e.EscultorId == id);
        }

        public Escultor Update(Escultor escultor)
        {
            _context.Escultores.Update(escultor);
            _context.SaveChanges();
            return escultor;
        }

        public Escultor? Delete(int id)
        {
            var escultor = _context.Escultores.Single(e => e.EscultorId == id);
            _context.Escultores.Remove(escultor);
            _context.SaveChanges();
            return escultor;
        }
    }
}

