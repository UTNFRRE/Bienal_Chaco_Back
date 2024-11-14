using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EsculturaPostPut
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = "";
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        [Required]
        public IFormFile? Imagen { get; set; }
        //primero sin control de que exista ese escultor
        [Required]
        public int EscultorID { get; set; }

        //desde aca va el GetAll
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? Tematica { get; set; }
        [Required]
        public int EdicionAño { get; set; }
    }

    public class EsculturaPatch
    {  
        [MaxLength(50)]
        public string? Nombre { get; set; }
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        public IFormFile? Imagen { get; set; }
        public int? EscultorID { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public string? Tematica { get; set; }
    }

    public class EsculturaVoto
    {
        public int Voto { get; set; }
    }
}
