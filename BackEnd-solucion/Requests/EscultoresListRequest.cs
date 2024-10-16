﻿using Microsoft.AspNetCore.Http;
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
        public string? Telefono { get; set; }
        public string? Biografia { get; set; } = string.Empty;
        public IFormFile? Imagen { get; set; }
    }
    public class EscultoresPatchRequest
    {
        [MaxLength(20)]
        public string? Nombre { get; set; } = string.Empty;
        [MaxLength(20)]
        public string? Apellido { get; set; } = string.Empty;
        [MaxLength(8)]
        public string? DNI { get; set; } = string.Empty;
        public string? Pais { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Biografia { get; set; } = string.Empty;
        public IFormFile? Imagen { get; set; }
    }
}
