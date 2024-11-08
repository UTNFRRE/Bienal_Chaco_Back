using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Entidades
{
    public class Votos
    {
        [Key]
        public int VotoId { get; set; }
        [ForeignKey("MyUser")]
        [Required]
        public int UrserId { get; set; }

        [ForeignKey("Esculturas")]
        [Required]
        public int EsculturaId { get; set; }

        [Required]
        public float Puntuacion { get; set; }
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}