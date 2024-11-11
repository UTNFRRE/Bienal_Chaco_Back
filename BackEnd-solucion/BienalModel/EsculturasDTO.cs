using Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Models
{
    public class EsculturasDetailDTO
    {
        public int EsculturaId { get; set; }
        public string Nombre { get; set; }
        public string? Tematica { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public string? EscultorNombre { get; set; }
        public string? EscultorPais { get; set; }
        public double promedioVotos { get; set; }

        [JsonIgnore]
        private string urlBase = "https://bienalobjectstorage.blob.core.windows.net/imagenes/";

        // Cambiamos de string a List<string> para manejar múltiples URLs de imágenes
        public List<string> Imagenes { get; set; } = new List<string>();

        public EsculturasDetailDTO(Esculturas escultura, Escultores escultor)
        {
            EsculturaId = escultura.EsculturaId;
            Nombre = escultura.Nombre;
            Tematica = escultura.Tematica;
            Descripcion = escultura.Descripcion;
            FechaCreacion = escultura.FechaCreacion;
            EscultorNombre = escultor.Nombre;
            EscultorPais = escultor.Pais;
            promedioVotos = escultura.PromedioVotos;

            // Agregamos todas las URLs de fotos
            foreach (var imagen in escultura.Imagenes)
            {
                Imagenes.Add(urlBase + imagen.Url);
            }
        }
    }

    public class EsculturasListLiteDTO
    {
        public int EsculturaId { get; set; }
        public string Nombre { get; set; }
        public string? Tematica { get; set; }
        public string? Descripcion { get; set; }
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string? EscultorNombre { get; set; }
        public string? EscultorPais { get; set; }
        public double promedioVotos { get; set; }

        [JsonIgnore]
        private string urlBase = "https://bienalobjectstorage.blob.core.windows.net/imagenes/";

        // Cambiamos de string a List<string> para manejar múltiples URLs de imágenes
        public List<string> Imagenes { get; set; } = new List<string>();

        public EsculturasListLiteDTO(Esculturas escultura, Escultores escultor)
        {
            EsculturaId = escultura.EsculturaId;
            Nombre = escultura.Nombre;
            Tematica = escultura.Tematica;
            Descripcion = escultura.Descripcion;
            FechaCreacion = escultura.FechaCreacion;
            EscultorNombre = escultor.Nombre + escultor.Apellido;
            EscultorPais = escultor.Pais;
            promedioVotos = escultura.PromedioVotos;

            // Agregamos todas las URLs de fotos
            foreach (var imagen in escultura.Imagenes)
            {
                Imagenes.Add(urlBase + imagen.Url);
            }
        }
    }
}
