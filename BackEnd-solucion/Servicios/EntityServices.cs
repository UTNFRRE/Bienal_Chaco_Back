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
    public class EsculturasServices : ICRUDService
    {
        private BienalDbContext _context;
        private IAzureStorageService _azureStorageService;

        public EsculturasServices(BienalDbContext context, IAzureStorageService azureStorageService)
        {
            this._context = context;
            this._azureStorageService = azureStorageService;
        }

        public async Task<Esculturas> CreateAsync(EsculturaListRequest request) //cambiar por esculturaRequest
        {
            var newEscultura = new Esculturas()
            {
                Nombre = request.Nombre,
                EscultorID = request.EscultorID,
                EventoID = request.EventoID,
            };

            if (newEscultura.Imagenes != null) //cambiar por lo que viene en el request
            {
                newEscultura.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen);
            }

            this._context.Esculturas.Add(newEscultura);
            this._context.SaveChangesAsync();

            return newEscultura;
        }

        public async Task<IEnumerable<Esculturas>> GetAllAsync()
        {
            return this._context.Esculturas.ToList();
        }

        public async Task<Esculturas> GetByAsync(int id)
        {
            return this._context.Esculturas.Find(id);
        }

        public async Task<Esculturas> UpdateAsync(int id, EsculturaListRequest request)
        {
            var esculturaToUpdate = this._context.Esculturas.Find(id);
            if (esculturaToUpdate != null)

            {
                esculturaToUpdate.Nombre = request.Nombre;
                esculturaToUpdate.EscultorID = request.EscultorID;
                esculturaToUpdate.EventoID = request.EventoID;

                if (esculturaToUpdate.Imagenes != null)
                {
                    esculturaToUpdate.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen);
                }
                
                this._context.Update(esculturaToUpdate);
                this._context.SaveChangesAsync();
            }
            
            return esculturaToUpdate;
        }

        public async Task DeleteAsync(int id)
        {
            var esculturaToDelete = this._context.Esculturas.Find(id);

            if (esculturaToDelete != null)
            {
                if (!string.IsNullOrEmpty(esculturaToDelete.Imagenes)) ;
                {
                    await this._azureStorageService.DeleteAsync(esculturaToDelete.Imagenes);
                }
                this._context.Esculturas.Remove(esculturaToDelete);
                this._context.SaveChangesAsync();
            }
        }

    }

    public interface ICRUDService
    { 
        Task<Esculturas> CreateAsync(EsculturaListRequest request);
        Task<IEnumerable<Esculturas>> GetAllAsync();
        Task<Esculturas> GetByAsync(int id); 
        Task<Esculturas> UpdateAsync(int id, EsculturaListRequest request);
        Task DeleteAsync(int id);
    } 
}
