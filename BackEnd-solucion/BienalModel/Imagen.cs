using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Entidades
{
    public class Imagen
    {
        [JsonIgnore]
        public string? FilePath { get; set; } // Ruta de almacenamiento (si aplica)
        public string Url { get; set; } // Ruta de almacenamiento (si aplica)
        public int Id { get; set; }
        [JsonIgnore]
        public string NombreArchivo { get; set; }
        public int EsculturaId { get; set; } // Relación con la escultura

        [JsonIgnore]
        public Esculturas Escultura { get; set; } // Relación inversa
    }
}