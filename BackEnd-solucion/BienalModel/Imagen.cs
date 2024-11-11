using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entidades
{
    public class Imagen
    {
        [Key]
        public int ImagenId { get; set; }

        [Required]
        public string Url { get; set; } = "";

        // Relación con Esculturas
        [ForeignKey("Escultura")]
        public int EsculturaId { get; set; }
        public Esculturas? Escultura { get; set; }
    }
}