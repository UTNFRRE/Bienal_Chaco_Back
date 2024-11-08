using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contexts;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Requests; // Para usar Entity Framework Core, especialmente consultas y manipulación de datos.

namespace Servicios
{
    public class VotosService: ICRUDServicesVotos
    {
        private readonly BienalDbContext _context;
        public VotosService(BienalDbContext context)
        {
            _context = context;
        }

        //metodo asincrono para crear un nuevo voto
        public async Task<Votos> CreateAsync(VotosListRequest request)
        {
            Votos newVoto = new Votos
            {
                UrserId = request.UrserId,
                EsculturaId = request.EsculturaId,
                Puntuacion = request.Puntuacion
            };
            _context.Votos.Add(newVoto);
            await _context.SaveChangesAsync();
            return newVoto;
        }

        //Metodo asincrono para obtener todos los votos (pruebas)
        public async Task<IEnumerable<Votos>> GetAllAsync()
        {
            return await _context.Votos.ToListAsync();
        }
    }
    public interface ICRUDServicesVotos
    {
        Task <Votos> CreateAsync(VotosListRequest votos);
        
        Task<IEnumerable<Votos>> GetAllAsync();
    }
}
