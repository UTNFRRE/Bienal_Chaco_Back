using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Requests;
using Contexts;

namespace Servicios
{
    public class EventosServices : ICRUDServiceEvent
    {
        private BienalDbContext _context;

        public EventosServices(BienalDbContext context)
        {
            this._context = context;
        }

        public async Task<Eventos> CreateEventoAsync(EventoCreateRequest request)
        {
            var newEvento = new Eventos
            {
                Nombre = request.Nombre,
                Fecha = request.Fecha,
                Lugar = request.Lugar,
                Descripcion = request.Descripcion,
                Tematica = request.Tematica,
                latitud = request.latitud,
                longitud = request.longitud
            };
            
            
            
            this._context.Eventos.Add(newEvento);
            await this._context.SaveChangesAsync();
            return newEvento;
        }

        // Obtener todos los eventos
        public async Task<IEnumerable<Eventos>> GetAllEventosAsync()
        {
            var listaEventos = await this._context.Eventos.ToListAsync();
            return listaEventos;
        }

        // Obtener un evento por su ID
        public async Task<Eventos> GetEventoByIdAsync(int id)
        {
            return await this._context.Eventos.FindAsync(id);
        }

        // Actualizar un evento existente
        public async Task<Eventos> UpdateEventoAsync(int id, EventoUpdateRequest request)
        {
            var eventoExistente = await this._context.Eventos.FindAsync(id);
            if (eventoExistente != null)
            {
                eventoExistente.Nombre = request.Nombre;
                eventoExistente.Fecha = request.Fecha;
                eventoExistente.Lugar = request.Lugar;
                eventoExistente.Descripcion = request.Descripcion;
                eventoExistente.Tematica = request.Tematica;
                eventoExistente.latitud = request.latitud;
                eventoExistente.longitud = request.longitud;

                await this._context.SaveChangesAsync();
            }
            return eventoExistente;
        }

        public async Task<Eventos> UpdatePatchEventoAsync( int id, EventoPatchRequest request)
        {
            var eventoExistente = await this._context.Eventos.FindAsync(id);
            if (eventoExistente != null)
            {
                eventoExistente.Nombre = request.Nombre ?? eventoExistente.Nombre;
                eventoExistente.Fecha = request.Fecha ?? eventoExistente.Fecha ;
                eventoExistente.Lugar = request.Lugar ?? eventoExistente.Lugar;
                eventoExistente.Descripcion = request.Descripcion ?? eventoExistente.Descripcion;
                eventoExistente.Tematica = request.Tematica ?? eventoExistente.Tematica;
                eventoExistente.latitud = request.latitud ?? eventoExistente.latitud;
                eventoExistente.longitud = request.longitud ?? eventoExistente.longitud;

                await this._context.SaveChangesAsync();
            }
            return eventoExistente;


        }
            

        // Eliminar un evento
        public async Task DeleteEventoAsync(int id)
        {
            var eventoTODelete = await this._context.Eventos.FindAsync(id);

            if (eventoTODelete != null)
            {
                this._context.Eventos.Remove(eventoTODelete);
                await this._context.SaveChangesAsync();
            }
        }

        // Verificar si un evento existe
        private async Task<bool> EventoExists(int id)
        {
            return await _context.Eventos.AnyAsync(e => e.Id == id);
        }
    }

    public interface ICRUDServiceEvent
    {
        
        Task<Eventos> CreateEventoAsync(EventoCreateRequest request);
        Task<IEnumerable<Eventos>> GetAllEventosAsync();
        Task<Eventos> GetEventoByIdAsync(int id);
        Task<Eventos> UpdateEventoAsync(int id,EventoUpdateRequest request);
        Task<Eventos> UpdatePatchEventoAsync(int id,EventoPatchRequest request);
        Task DeleteEventoAsync(int id);
    }


}
