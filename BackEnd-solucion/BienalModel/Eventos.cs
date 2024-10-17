
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
        public class Eventos
       {
        [Key]   //Para hacer un post pide necesariamente el id, seria interesante que no lo requiera
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(200)]
        public string Lugar { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public string Tematica { get; set; }

        public double? latitud { get; set; }
        public double? longitud {get; set; }
    }
}

