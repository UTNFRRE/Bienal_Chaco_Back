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
        [MaxLength(100)]
        public string Nombre { get; set; }
        
        [Required]
        public int EscultorID { get; set; }
       
        public int EventoID { get; set; }
        
        public string Imagenes { get; set; }
    }
}
