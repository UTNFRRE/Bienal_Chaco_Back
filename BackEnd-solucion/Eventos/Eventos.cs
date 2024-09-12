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
        public EventosContext(DbContextOptions<EventosContext> options) : base(options)
        {
        }

        // Define la tabla de eventos
        public DbSet<Eventos> Eventos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Aquí puedes agregar configuraciones adicionales si es necesario
        }
    }

    public class EventosServices
    {
        private readonly EventosContext _context;

        public EventosServices(EventosContext context)
        {
            _context = context;
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
        public async Task<bool> UpdateEventoAsync(Eventos evento)
        {
            _context.Entry(evento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventoExists(evento.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        // Eliminar un evento
        public async Task<bool> DeleteEventoAsync(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return false;
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return true;
        }

        // Verificar si un evento existe
        private async Task<bool> EventoExists(int id)
        {
            return await _context.Eventos.AnyAsync(e => e.Id == id);
        }
    }
}
