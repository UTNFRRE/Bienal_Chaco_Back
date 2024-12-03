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



        // multi imagenes
        [Required]
        // Relación de uno a muchos con las imágenes
        public ICollection<Imagen> Imagenes { get; set; }





        //primero sin control de que exista ese escultor
        [Required]
        public int EscultoresID { get; set; }
        
        //desde aca va el GetAll
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? Tematica { get; set; }

        public int CantVotaciones { get; set; }
        //navegabilidad para relacion muchos a muchos con votos
        [JsonIgnore]
        public virtual ICollection<Votos>? Votos { get; set; }
        //atributo cal ulado Votos/CantVotaciones
        public double PromedioVotos { get; set; }

        [ForeignKey("Edicion")]
        public int EdicionAño { get; set; }

    }
}
