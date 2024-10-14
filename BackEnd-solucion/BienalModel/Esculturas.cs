using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Esculturas
    {
        [Key]
        public int EsculturaId { get; set; }
        [Required]
        [MaxLength(20)]
        public string Nombre { get; set; } = "";
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        [Required]
        public string Imagenes { get; set; } = "";
        //primero sin control de que exista ese escultor
        [Required]
        public int EscultorID { get; set; }
        
        //desde aca va el GetAll
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? Tematica { get; set; }
   
    }
}
