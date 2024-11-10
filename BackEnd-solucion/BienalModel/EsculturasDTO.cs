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

        public string Imagenes { get; set; }
        public double promedioVotos { get; set; }

        //constructor de Esculturas
        public EsculturasDetailDTO(Esculturas escultura, Escultores Escultor) 
              {
            EsculturaId = escultura.EsculturaId;
            Nombre = escultura.Nombre;
            Tematica = escultura.Tematica;
            Descripcion = escultura.Descripcion;
            FechaCreacion = escultura.FechaCreacion;
            Imagenes = urlImagen + escultura.Imagenes;
            


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
        [JsonIgnore]
        public string urlImagen { get; set; } = "https://bienalobjectstorage.blob.core.windows.net/imagenes/";

        public string Imagenes { get; set; }
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
            Imagenes = urlImagen + escultura.Imagenes;
            promedioVotos = escultura.PromedioVotos;
        }



    }
}
