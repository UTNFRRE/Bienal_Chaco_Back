using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Requests
{
    public class VotosListRequest
    {

        [Required]
        public int UrserId { get; set; }
        [Required]
        public int EsculturaId { get; set; }
        [Required]
        public float Puntuacion { get; set; }

    }
}
