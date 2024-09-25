using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BienalModel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BienalModel
{
    public class BienalDbContext : DbContext
    {
        public DbSet<Escultor> Escultores { get; set; }
        public DbSet<Escultura> Esculturas { get; set; }
        public DbSet<Eventos> Eventos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // InMemoryDatabase para pruebas, después cambiar a SQL Server
            optionsBuilder.UseInMemoryDatabase("BienalDB");
            // optionsBuilder.UseSqlServer(@"Server=.\;Database=BienalDB;Trusted_Connection=True;");
        }
    }

    /* ESCULTORES */

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
        public string? Foto { get; set; }

        public ICollection<Escultura> Esculturas { get; } = new List<Escultura>();
    }

    public class EscultorService
    {
        private BienalDbContext _contextEscultor;

        public EscultorService() // Constructor sin parámetros de escultor service 
        {
            _contextEscultor = new BienalDbContext();
        }

        public async Task<Escultor> Create(Escultor escultor) //Service method to create a new escultor
        {
            _contextEscultor.Escultores.Add(escultor);
            await _contextEscultor.SaveChangesAsync();
            return escultor;
        }

        public async Task<IEnumerable<Escultor>> GetAll() //Service method para obtener todos los escultores
        {
            var lista = await _contextEscultor.Escultores.ToListAsync();
            return lista;
        }

        public async Task<Escultor?> GetById(int id) //Service method obtener escultor por id
        {
            return await _contextEscultor.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
        }

        public async Task<Escultor> Update(Escultor escultor) //service method para actualizar un escultor
        {
            _contextEscultor.Escultores.Update(escultor);
            await _contextEscultor.SaveChangesAsync();
            return escultor;
        }

        public async Task<Escultor?> Delete(int id) //service method para eliminar un escultor
        {
            var escultor = await _contextEscultor.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
            if (escultor != null)
            {
                _contextEscultor.Escultores.Remove(escultor);
                await _contextEscultor.SaveChangesAsync();
            }
            return escultor;
        }
    }

    /* ESCULTURAS */

    public class Escultura
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
        [JsonIgnore]
        public Escultor Escultor { get; set; }
        public List<string> Imagenes { get; set; }
    }

    public class imagenService
    {
        public IFormFile Archivo { get; set; }
    }

    public class EsculturasServices
    {
        // Se espera que esta clase proporcione servicios relacionados con las esculturas, conoce el contexto y es el intermediario entre la clase Esculturas y la Base de Datos. 
        // Aquí se definen los métodos para realizar operaciones CRUD (crear, leer, actualizar, eliminar) en las esculturas.
        private readonly BienalDbContext _contextEscultura;

        public EsculturasServices()
        {
            _contextEscultura = new BienalDbContext();
        }

        // Crear
        public async Task CrearEscultura(Escultura escultura)
        {
            escultura.UltimaModificacion = DateTime.Now;
            _contextEscultura.Esculturas.Add(escultura);
            await _contextEscultura.SaveChangesAsync();
        }

        // Leer
        public async Task<Escultura?> ObtenerEscultura(int id)
        {
            return await _contextEscultura.Esculturas.FindAsync(id) ?? null;
        }
        //leer todo
        public async Task<List<Escultura>> ObtenerTodasLasEsculturas()
        {
            return await _contextEscultura.Esculturas.ToListAsync();
        }
        // Actualizar
        public async Task ActualizarEscultura(Escultura escultura)
        {
            var esculturaExistente = await _contextEscultura.Esculturas.FindAsync(escultura.EsculturasID);
            if (esculturaExistente != null)
            {
                esculturaExistente.Nombre = escultura.Nombre;
                esculturaExistente.Tematica = escultura.Tematica;
                esculturaExistente.Fecha = escultura.Fecha;
                esculturaExistente.EscultorID = escultura.EscultorID;
                esculturaExistente.EventoID = escultura.EventoID;
                esculturaExistente.UltimaModificacion = DateTime.Now;

                await _contextEscultura.SaveChangesAsync();
            }
        }

        // Eliminar
        public async Task EliminarEscultura(int id)
        {
            var escultura = await _contextEscultura.Esculturas.FindAsync(id);
            if (escultura != null)
            {
                _contextEscultura.Esculturas.Remove(escultura);
                await _contextEscultura.SaveChangesAsync();
            }
        }
    }
}

public class Eventos
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Nombre { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    [StringLength(200)]
    public string Lugar { get; set; }

    [Required]
    public string Descripcion { get; set; }

    [Required]
    [StringLength(100)]
    public string Tematica { get; set; }
}
public class EventosServices
{
    private readonly BienalDbContext _contextEvento;

    public EventosServices()
    {
        _contextEvento = new BienalDbContext();
    }

    // Obtener todos los eventos
    public async Task<IEnumerable<Eventos>> GetAllEventosAsync()
    {
        return await _contextEvento.Eventos.ToListAsync();
    }

    // Obtener un evento por su ID
    public async Task<Eventos> GetEventoByIdAsync(int id)
    {
        return await _contextEvento.Eventos.FindAsync(id);
    }

    // Crear un nuevo evento
    public async Task<Eventos> CreateEventoAsync(Eventos evento)
    {
        _contextEvento.Eventos.Add(evento);
        await _contextEvento.SaveChangesAsync();
        return evento;
    }

    // Actualizar un evento existente
    public async Task<Eventos> UpdateEventoAsync(Eventos evento)
    {
        var eventoExistente = await _contextEvento.Eventos.FindAsync(evento.Id);
        if (eventoExistente != null)
        {
            eventoExistente.Nombre = evento.Nombre;
            eventoExistente.Fecha = evento.Fecha;
            eventoExistente.Lugar = evento.Lugar;
            eventoExistente.Descripcion = evento.Descripcion;
            eventoExistente.Tematica = evento.Tematica;

            await _contextEvento.SaveChangesAsync();
        }
        return eventoExistente;
    }

    // Eliminar un evento
    public async Task<Eventos> DeleteEventoAsync(int id)
    {
        var eventoEliminado = await _contextEvento.Eventos.FindAsync(id);
        _contextEvento.Eventos.Remove(eventoEliminado);
        await _contextEvento.SaveChangesAsync();
        return eventoEliminado;
    }

    // Verificar si un evento existe
    private async Task<bool> EventoExists(int id)
    {
        return await _contextEvento.Eventos.AnyAsync(e => e.Id == id);
    }
}