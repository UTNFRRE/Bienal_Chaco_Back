﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string Imagenes { get; set; } = "";
        //primero sin control de que exista ese escultor
        [Required]
        public int EscultoresID { get; set; }
        
        //desde aca va el GetAll
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? Tematica { get; set; }

        public int CantVotaciones { get; set; }
        [JsonIgnore]
        public int Votos { get; set; }
        //atributo cal ulado Votos/CantVotaciones
        public double PromedioVotos { get; set; }
   
    }
}
