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
        public string? EscultorImagen { get; set; }
        [JsonIgnore]
        public string urlImagen { get; set; } = "https://bienalobjectstorage.blob.core.windows.net/imagenes/";

        // List<Imagen> mantiene la propiedad original
        public List<Imagen> Imagenes { get; set; }

        // Nueva propiedad para devolver las URLs de las imágenes
        public List<string> ImagenesUrls { get; set; }

        public double promedioVotos { get; set; }

        //constructor de Esculturas
        public EsculturasDetailDTO(Esculturas escultura, Escultores Escultor) 
            {
                EsculturaId = escultura.EsculturaId;
                Nombre = escultura.Nombre;
                Tematica = escultura.Tematica;
                Descripcion = escultura.Descripcion;
                FechaCreacion = escultura.FechaCreacion;

                // Asignar la lista original de imágenes
                Imagenes = escultura.Imagenes;

                // Generar URLs dinámicamente
                ImagenesUrls = escultura.Imagenes
                    .Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo)
                    .ToList();

                EscultorNombre = Escultor.Nombre;
                EscultorImagen = urlImagen + Escultor.Foto;
                EscultorPais = Escultor.Pais;
                promedioVotos = escultura.PromedioVotos;
            }
    }

    public class EsculturasListLiteDTO
    {
        public int EsculturaId { get; set; }
        public string Nombre { get; set; }
        public string? Tematica { get; set; }
        public string? Descripcion { get; set; }

        public int EscultorID { get; set;}

        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? EscultorNombre { get; set; }

        public string? EscultorPais { get; set; }

        public List<Imagen> Imagenes { get; set; }

        public List<string> ImagenesUrls { get; set; }

        public double promedioVotos { get; set; }

        //constructor de Esculturas
        public EsculturasListLiteDTO(Esculturas escultura, Escultores escultor)
        {
            EsculturaId = escultura.EsculturaId;
            Nombre = escultura.Nombre;
            Tematica = escultura.Tematica;
            Descripcion = escultura.Descripcion;
            EscultorID = escultura.EscultoresID;
            FechaCreacion = escultura.FechaCreacion;
            EscultorNombre = escultor.Nombre + escultor.Apellido;
            EscultorPais = escultor.Pais;
            Imagenes = escultura.Imagenes;
            // Generar URLs dinámicamente
            ImagenesUrls = escultura.Imagenes
                .Select(img => "https://bienalobjectstorage.blob.core.windows.net/imagenes/" + img.NombreArchivo)
                .ToList();
            promedioVotos = escultura.PromedioVotos;
        }
    }
}
