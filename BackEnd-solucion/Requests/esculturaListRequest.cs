using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class EsculturaListRequest
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public int EscultorID { get; set; }
        [Required]
        public int EventoID { get; set; }
        public IFormFile? Imagen { get; set; }
    }
}
