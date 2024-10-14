using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requests
{
    public class EscultoresListRequest
    {
        [Required]
        public string? Nombre { get; set; } = string.Empty;
        [Required]
        public string? Apellido { get; set; } = string.Empty;
        [Required]
        public string? DNI { get; set; } = string.Empty;
        [Required]
        public string? Pais { get; set; } = string.Empty;
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Contraseña { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Biografia { get; set; } = string.Empty;
        public IFormFile? Imagen { get; set; }
    }
}
