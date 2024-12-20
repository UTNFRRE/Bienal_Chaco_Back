
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EventoCreateRequest
    {

        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Lugar { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Tematica { get; set; }
        public double? longitud { get; set; }
        public double? latitud { get; set; }
        [Required]
        public int EdicionAņo { get; set; }
    }

    public class EventoUpdateRequest
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public string Lugar { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public string Tematica { get; set; }
        [Required]
        public double longitud { get; set; }
        [Required]
        public double latitud { get; set; }
    }

    public class EventoPatchRequest
    {
        public string? Nombre { get; set; }

        public DateTime? Fecha { get; set; }

        public string? Lugar { get; set; }
     
        public string? Descripcion { get; set; }

        public string? Tematica { get; set; }
        public double? longitud { get; set; }
        public double? latitud { get; set; }
    }

}

