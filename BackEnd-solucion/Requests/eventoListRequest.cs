
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EventoCreateRequest
    {
        // No es necesario el Id para el POST (se genera automáticamente)
        public int? Id { get; set; }

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
    }

    public class EventoUpdateRequest
    {
        [Required]
        public int Id { get; set; } // Ahora es requerido para el UPDATE

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
    }
}

