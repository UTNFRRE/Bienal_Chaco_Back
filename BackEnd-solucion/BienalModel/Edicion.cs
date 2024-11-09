using Entidades;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Edicion
    {

        [Key]
        public int Año { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public ICollection<Eventos>? Eventos { get; set; }

        public ICollection<Escultores>? Escultores { get; set; }

        public ICollection<Esculturas>? Esculturas { get; set; }

    }
}
