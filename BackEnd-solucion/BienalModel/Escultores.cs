using Entidades;
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
        [MaxLength(50)]
        public string? Pais { get; set; }
        public string? Telefono { get; set; }
        [MaxLength(255)]
        public string? Biografia { get; set; }
        public string? Foto { get; set; }
        [JsonIgnore]
        public ICollection<Esculturas>? Esculturas { get; set; }
        [ForeignKey("Edicion")]
        public int EdicionAño { get; set;}
    }
}
