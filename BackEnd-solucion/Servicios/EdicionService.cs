using Contexts;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Models;
using Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class Ediciones
    {
        public class EdicionServices : ICRUDServiceEdicion
        {
            private BienalDbContext _context;

            public EdicionServices(BienalDbContext context)
            {
                this._context = context;
            }

            public async Task<Edicion> CreateEdicionAsync(EdicionPostRequests request)
            {
                var newEdicion = new Edicion
                {
                    Año = request.Año,
                    FechaInicio = request.FechaInicio,
                    FechaFin = request.FechaFin
                };

                this._context.Edicion.Add(newEdicion);
                await this._context.SaveChangesAsync();
                return newEdicion;
            }

            public async Task<IEnumerable<Edicion>> GetAllEdicionAsync()
            {
                var listaEdicion = await this._context.Edicion.ToListAsync();
                return listaEdicion;
            }

            // Obtener un evento por su ID
            public async Task<Edicion> GetEdicionByAño(int Año)
            {
                return await this._context.Edicion.FindAsync(Año);
            }
        
            // Actualizar un evento existente
            public async Task<Edicion> UpdateEdicionAsync(int año, EdicionUpdateRequest request)
            {
                var edicionExistente = await this._context.Edicion.FindAsync(año);
                if (edicionExistente != null)
                {
                    edicionExistente.Año = request.Año;
                    edicionExistente.FechaInicio = request.FechaInicio;
                    edicionExistente.FechaFin = request.FechaFin;
                    
                    this._context.Update(edicionExistente);

                    await this._context.SaveChangesAsync();
                }
                return edicionExistente;
            }

            public async Task<Edicion> UpdatePatchEdicionAsync(int año, EdicionPatchEdicion request)
            {
                var edicionExistente = await this._context.Edicion.FindAsync(año);
                if (edicionExistente != null)
                {
                    if (request.Año != null)
                    {
                        edicionExistente.Año = request.Año;
                    }
                   
                    if (request.FechaInicio != null)
                    {
                        edicionExistente.FechaInicio = request.FechaInicio;
                    }
            

                    if (request.FechaFin != null)
                    {
                        edicionExistente.FechaFin = request.FechaFin;
                    }
                    this._context.Update(edicionExistente);

                    await this._context.SaveChangesAsync();
                }
                return edicionExistente;


            }

            // Eliminar un evento
            public async Task<bool> DeleteEdicionAsync(int año)
            {
                var edicionToDelete = await this._context.Edicion.FindAsync(año);

                if (edicionToDelete != null)
                {
                    this._context.Edicion.Remove(edicionToDelete);
                    await this._context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            
            
        }

        public interface ICRUDServiceEdicion
        {

            Task<Edicion> CreateEdicionAsync(EdicionPostRequests request);

            Task<IEnumerable<Edicion>> GetAllEdicionAsync();

            Task<Edicion> GetEdicionByAño(int año);

            Task<Edicion> UpdateEdicionAsync(int año, EdicionUpdateRequest request);

            Task<Edicion> UpdatePatchEdicionAsync(int año, EdicionPatchEdicion request);

            Task<bool> DeleteEdicionAsync(int año);

        }
    }
}
