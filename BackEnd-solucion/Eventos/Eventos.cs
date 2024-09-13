using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace EventosModel
{
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

    public class EventosContext : DbContext
    {
        public DbSet<Eventos> Eventos { get; set; }
        //public EventosContext(DbContextOptions<EventosContext> options) : base(options)
        //{
        //}

        // Define la tabla de eventos

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);
        // Aquí puedes agregar configuraciones adicionales si es necesario
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TallerMecanicoDB");
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Taller;Trusted_Connection=True;");
        }
    }

    public class EventosServices
    {
        private readonly EventosContext _context;

        public EventosServices()
        {
            //_context = context;
            //te falto crear la instancia de EventosContext
            _context = new EventosContext();
        }

        // Obtener todos los eventos
        public async Task<IEnumerable<Eventos>> GetAllEventosAsync()
        {
            return await _context.Eventos.ToListAsync();
        }

        // Obtener un evento por su ID
        public async Task<Eventos> GetEventoByIdAsync(int id)
        {
            return await _context.Eventos.FindAsync(id);
        }

        // Crear un nuevo evento
        public async Task<Eventos> CreateEventoAsync(Eventos evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        // Actualizar un evento existente
        public async Task<Eventos> UpdateEventoAsync(Eventos evento)
        {
            await _context.SaveChangesAsync();
            return evento;
            
            /*
            _context.Entry(evento).State = EntityState.Modified;
             Control de Errores que despues va a servir
            try
            {
                await _context.SaveChangesAsync();
                return evento;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventoExists(evento.Id))
                {
                    return evento;
                }
                else
                {
                    throw;
                }
            }
            */
        }

        // Eliminar un evento
        public async Task<Eventos> DeleteEventoAsync(int id)
        {
            var eventoEliminado = await _context.Eventos.FindAsync(id);
            _context.Eventos.Remove(eventoEliminado);
            await _context.SaveChangesAsync();
            return eventoEliminado;
        }

        // Verificar si un evento existe
        private async Task<bool> EventoExists(int id)
        {
            return await _context.Eventos.AnyAsync(e => e.Id == id);
        }
    }
}
