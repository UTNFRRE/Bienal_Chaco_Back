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

        public string Nombre { get; set; }
      
        public int EscultorID { get; set; }
        
        public int EventoID { get; set; }
        public string Imagenes { get; set; }
    }
}
