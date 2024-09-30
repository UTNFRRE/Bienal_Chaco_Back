using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using BienalModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Servicios
{
    public class EsculturasServices : ICRUDService<Escultura>
    {
        private BienalDbContext _context;
        private IAzureStorageService azureStorageService;

        public EsculturasServices()
        {
            _context = new BienalDbContext();
            azureStorageService = new AzureBlobStorageService();
        }

        public async Task<Escultura> CreateAsync(Escultura escultura)
        {
            var newEscultura = new Escultura()
            {
                Nombre = escultura.Nombre,
                Tematica = escultura.Tematica,
                Fecha = escultura.Fecha,
                EscultorID = escultura.EscultorID,
                EventoID = escultura.EventoID,
            };

            if (escultura.Imagenes != null)
            {
                escultura.Imagenes = await this.azureStorageService.UploadAsync(escultura.Imagenes, car.ImagePath);
            }
            _context.Esculturas.Add(newEscultura);
            await _context.SaveChangesAsync();

            return newEscultura();
        }

        public async Task<IEnumerable<Escultura>> GetAllAsync()
        {
            return _context.Esculturas.ToList();
        }

        public async Task<Escultura> GetByAsync(int id)
        {
            return _context.Esculturas.Find(id);
        }

        public async Task<Escultura> UpdateAsync(int id ,Escultura escultura)
        {
            var esculturaToUpdate = _context.Esculturas.Find(id);
            if (esculturaToUpdate != null)
            
            {
                esculturaToUpdate.Nombre = escultura.Nombre;
                esculturaToUpdate.Tematica = escultura.Tematica;
                esculturaToUpdate.Fecha = escultura.Fecha;
                esculturaToUpdate.EscultorID = escultura.EscultorID;
                esculturaToUpdate.EventoID = escultura.EventoID;
            }
            if (escultura.Imagenes != null)
            {
                esculturaToUpdate.Imagenes = await this.azureStorageService.UploadAsync(escultura.Imagenes, car.ImagePath);
            }
            
            await _context.SaveChangesAsync();
            return esculturaToUpdate;
        }

        public async Task DeleteAsync(int id)
        {
            var esculturaToDelete = _context.Esculturas.Find(id);

            if (esculturaToDelete != null)
                {
                if (!string.IsNullOrEmpty(esculturaToDelete.Imagenes)) ;
                    {
                        await this.azureStorageService.DeleteAsync(esculturaToDelete.Imagenes);
                    }
            _context.Esculturas.Remove(esculturaToDelete);
            await _context.SaveChangesAsync();
        }
    }

    }

    public interface ICRUDService<T>
    { 
        Task<T> CreateAsync(T request);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByAsync(int id);
        Task<T> UpdateAsync(int id, T request);
        Task DeleteAsync(int id);
    }
}
