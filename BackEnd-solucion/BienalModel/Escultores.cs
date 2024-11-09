using Entidades;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entidades
{
    public class Escultores
    {
        [Key]
        public int EscultorId { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Nombre { get; set; }
        [Required]
        [MaxLength(30)]
        public string? Apellido { get; set; }
        [Required]
        [MaxLength(10)]
        public string? DNI { get; set; }

        public DateOnly FechaNacimiento { get; set; }
        [MaxLength(60)]
        public string? LugarNacimiento { get; set; }

        public string Premios { get; set; }

        [MaxLength(50)]
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        [MaxLength(255)]
        public string? Biografia { get; set; }
        public string? Foto { get; set; }
        [JsonIgnore]
        public ICollection<Esculturas>? Esculturas { get; set; }

    }
}
