using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Esculturas
{
    public class EsculturasContext : DbContext
    {
        // Se espera que esta clase represente el contexto de datos, es la encargada de gestionar el objeto y la conexión a la base de datos.
        // Aquí se pueden definir las tablas y configuraciones de la base de datos utilizando Entity Framework.
        public DbSet<EsculturasModel> Esculturas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("EsculturasDb");
        }
    }

    public class EsculturasModel
    {
        // Se espera que esta clase represente una escultura individual.
        // Acá se definen las propiedades de la escultura específica.
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EsculturasID { get; set; }

        public required string Nombre { get; set; }
        public string? Tematica { get; set; }
        public DateTime Fecha { get; set; }
        public int EscultorID { get; set; }
        public int EventoID { get; set; }
        public DateTime UltimaModificacion { get; set; }
    }

    public class imagenService
    {
        public IFormFile Archivo { get; set; }
            }
    public class EsculturasServices
    {
        // Se espera que esta clase proporcione servicios relacionados con las esculturas, conoce el contexto y es el intermediario entre la clase Esculturas y la Base de Datos. 
        // Aquí se definen los métodos para realizar operaciones CRUD (crear, leer, actualizar, eliminar) en las esculturas.
        private readonly EsculturasContext _context;

        public EsculturasServices()
        {
            _context = new EsculturasContext();
        }

        // Crear
        public async Task CrearEscultura(EsculturasModel escultura)
        {
            escultura.UltimaModificacion = DateTime.Now;
            _context.Esculturas.Add(escultura);
            await _context.SaveChangesAsync();
        }

        // Leer
        public async Task<EsculturasModel?> ObtenerEscultura(int id)
        {
            return await _context.Esculturas.FindAsync(id) ?? null;
        }
        //leer todo
        public async Task<List<EsculturasModel>> ObtenerTodasLasEsculturas()
        {
            return await _context.Esculturas.ToListAsync();
        }
        // Actualizar
        public async Task ActualizarEscultura(EsculturasModel escultura)
        {
            var esculturaExistente = await _context.Esculturas.FindAsync(escultura.EsculturasID);
            if (esculturaExistente != null)
            {
                esculturaExistente.Nombre = escultura.Nombre;
                esculturaExistente.Tematica = escultura.Tematica;
                esculturaExistente.Fecha = escultura.Fecha;
                esculturaExistente.EscultorID = escultura.EscultorID;
                esculturaExistente.EventoID = escultura.EventoID;
                esculturaExistente.UltimaModificacion = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }

        // Eliminar
        public async Task EliminarEscultura(int id)
        {
            var escultura = await _context.Esculturas.FindAsync(id);
            if (escultura != null)
            {
                _context.Esculturas.Remove(escultura);
                await _context.SaveChangesAsync();
            }
        }
    }


}
