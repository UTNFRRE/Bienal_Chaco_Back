﻿using System;
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
using Models; 

namespace Servicios
{
    public class EsculturasServices : ICRUDEsculturaService
    {
        private BienalDbContext _context;
        private IAzureStorageService _azureStorageService;

        public EsculturasServices(BienalDbContext context, IAzureStorageService azureStorageService)
        {
            this._context = context;
            this._azureStorageService = azureStorageService;
        }

        public async Task<Esculturas>? CreateAsync(EsculturaPostPut request) //cambiar por esculturaRequest
        {
            //validación si exise otra escultora con el mismo nombre
            var esculturaExistente = this._context.Esculturas.FirstOrDefault(e => e.Nombre == request.Nombre);
            
            if (esculturaExistente != null)
            {
                return null;
            }

            var newEscultura = new Esculturas()
            {
                Nombre = request.Nombre,
                EscultorID = request.EscultorID,
                Descripcion = request.Descripcion,
                FechaCreacion = request.FechaCreacion,
                Tematica = request.Tematica,
            };  

            if (request.Imagen!= null) //cambiar por lo que viene en el request
            {
                newEscultura.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen);
            }

           await this._context.Esculturas.AddAsync(newEscultura);
           await this._context.SaveChangesAsync();

            return newEscultura;

        }

        public async Task<IEnumerable<Esculturas>> GetAllAsync()
        {
            return await this._context.Esculturas.ToListAsync();
        }

        public async Task<IEnumerable<EsculturasListLiteDTO>> GetAllList()
        {
            var listescultura = await this._context.Esculturas.ToListAsync();
            var listesculturaDTO = new List<EsculturasListLiteDTO>();

            foreach (Esculturas esculturas in listescultura)
                {
                var escultor = await this._context.Escultores.FindAsync(esculturas.EscultorID);
                listesculturaDTO.Add(new EsculturasListLiteDTO(esculturas, escultor));
                }
            
            return listesculturaDTO;
        }

        public async Task<Esculturas>? GetByAsync(int id)
        {
            return await this._context.Esculturas.FindAsync(id);
        }

        public async Task<EsculturasDetailDTO>? GetDetail(int id)
        {
            var escultura = await this._context.Esculturas.FindAsync(id);
            
            if (escultura == null)
            {
                return null;
            }
            
            var escultor = await this._context.Escultores.FindAsync(escultura.EscultorID);
            EsculturasDetailDTO EsculturaDetalle = new EsculturasDetailDTO(escultura, escultor);
            return EsculturaDetalle;
            
        }

        ///modificar UpdateAsync para sobrecargar con parametro EsculturaPatchRequest
        public async Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPostPut request)
        {
            //validación si existe id del escultor

            /*var escultorExistente = this._context.Escultores.FirstOrDefault(e => e.EscultorId == request.EscultorID);
            if (esculturaExistente == null)
            {
                return null;
            }*/

        var esculturaToUpdate = this._context.Esculturas.Find(id);
            if (esculturaToUpdate != null)
            {
                esculturaToUpdate.Nombre = request.Nombre;
                esculturaToUpdate.EscultorID = request.EscultorID;
                esculturaToUpdate.Descripcion = request.Descripcion;
                esculturaToUpdate.FechaCreacion = request.FechaCreacion;
                esculturaToUpdate.Tematica = request.Tematica;

                //control de errores nuevo nombre de escultura no existe
               
                if (request.Imagen != null)
                {
                    esculturaToUpdate.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen, esculturaToUpdate.Imagenes);
                }
                
                this._context.Update(esculturaToUpdate);
                await this._context.SaveChangesAsync();
            }
            
            return esculturaToUpdate;
        }

        public async Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request)
        {
            var esculturaToUpdate = await this._context.Esculturas.FindAsync(id);

            if (esculturaToUpdate == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Nombre))
            {
                esculturaToUpdate.Nombre = request.Nombre;
            }


            if (!string.IsNullOrEmpty(request.Descripcion))
            {
                esculturaToUpdate.Descripcion = request.Descripcion;
            }

            if (request.Imagen != null)
            {
                esculturaToUpdate.Imagenes = await this._azureStorageService.UploadAsync(request.Imagen, esculturaToUpdate.Imagenes);
            }

            if (request.EscultorID != null)
            {
                //conversión implicita de int? a int
                esculturaToUpdate.EscultorID = (int)request.EscultorID;
            }

            if (request.FechaCreacion != null)
            {
                esculturaToUpdate.FechaCreacion = (DateOnly)request.FechaCreacion;
            }

            this._context.Update(esculturaToUpdate);

            await this._context.SaveChangesAsync();
            return esculturaToUpdate;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var esculturaToDelete = await this._context.Esculturas.FindAsync(id);

            if (esculturaToDelete == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(esculturaToDelete.Imagenes))
            {
                await this._azureStorageService.DeleteAsync(esculturaToDelete.Imagenes);
            }
            this._context.Esculturas.Remove(esculturaToDelete);
            await this._context.SaveChangesAsync();
            return true;

        }
    }

    }

    public interface ICRUDEsculturaService
    { 
        Task<Esculturas>? CreateAsync(EsculturaPostPut request);
        Task<IEnumerable<Esculturas>> GetAllAsync();
        Task<IEnumerable<EsculturasListLiteDTO>> GetAllList();
        Task<EsculturasDetailDTO>? GetDetail(int idEscultura);
        Task<Esculturas>? GetByAsync(int id); 
        Task<Esculturas>? UpdatePutEsculturaAsync(int id, EsculturaPostPut request);
        Task<Esculturas>? UpdatePatchAsync(int id, EsculturaPatch request);
        Task<bool> DeleteAsync(int id);
        
    } 

