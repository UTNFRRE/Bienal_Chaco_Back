using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contexts;
using Entidades;
using Microsoft.EntityFrameworkCore; // Para usar Entity Framework Core, especialmente consultas y manipulación de datos.

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
        public async Task<Votos> CreateAsync(Votos votos)
        {
            Votos newVoto = new Votos
            {
                UrserId = votos.UrserId,
                EsculturaId = votos.EsculturaId,
                Puntuacion = votos.Puntuacion
            };
            _context.Votos.Add(newVoto);
            await _context.SaveChangesAsync();
            return votos;
        }

        //Metodo asincrono para obtener todos los votos (pruebas)
        public async Task<IEnumerable<Votos>> GetAllAsync()
        {
            return await _context.Votos.ToListAsync();
        }
    }
    public interface ICRUDServicesVotos
    {
        Task <Votos> CreateAsync(Votos votos);
        
        Task<IEnumerable<Votos>> GetAllAsync();
    }
}
