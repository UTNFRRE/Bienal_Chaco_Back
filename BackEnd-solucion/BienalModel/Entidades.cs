using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace Entidades
{ }
    /* ESCULTORES */
/*
public class EscultorService
{
    private BienalDbContext _contextEscultor;

    public EscultorService() // Constructor sin parámetros de escultor service 
    {
        _contextEscultor = new BienalDbContext();
    }

    public async Task<Escultores> Create(Escultores escultor) //Service method to create a new escultor
    {
        _contextEscultor.Escultores.Add(escultor);
        await _contextEscultor.SaveChangesAsync();
        return escultor;
    }

    public async Task<IEnumerable<Escultores>> GetAll() //Service method para obtener todos los escultores
    {
        var lista = await _contextEscultor.Escultores.ToListAsync();
        return lista;
    }

    public async Task<Escultores?> GetById(int id) //Service method obtener escultor por id
    {
        return await _contextEscultor.Escultores.SingleOrDefaultAsync(e => e.EscultorId == id);
    }

    public async Task<Escultores> Update(Escultores escultor) //service method para actualizar un escultor
    {
        _contextEscultor.Escultores.Update(escultor);
        await _contextEscultor.SaveChangesAsync();
        return escultor;
    }

    public async Task<Escultores?> Delete(int id) //service method para eliminar un escultor
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
/*
}
*/