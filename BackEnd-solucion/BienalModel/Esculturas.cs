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
    public class Esculturas
    {
        [Key]
        public int EsculturaId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = "";

        [MaxLength(200)]
        public string? Descripcion { get; set; }

        // Cambiamos "Imagenes" a una colección de imagen
        public virtual ICollection<Imagen> Imagenes { get; set; } = new List<Imagen>();

        [Required]
        public int EscultoresID { get; set; }

        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string? Tematica { get; set; }
        public int CantVotaciones { get; set; }

        [JsonIgnore]
        public virtual ICollection<Votos>? Votos { get; set; }

        public double PromedioVotos { get; set; }

        [ForeignKey("Edicion")]
        public int EdicionAño { get; set; }
    }
}
