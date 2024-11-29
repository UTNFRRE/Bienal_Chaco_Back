using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requests
{
    public class EdicionPostRequests
    {
        [Required]
        public int Año { get; set; }

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public bool VotacionHabilitada { get; set; } = false;
    }

    public class EdicionUpdateRequest
    {
        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public bool VotacionHabilitada { get; set; } = false;

    }
    public class EdicionPatchEdicion
    {

        public DateOnly? FechaInicio { get; set; }

        public DateOnly? FechaFin { get; set; }

        public bool VotacionHabilitada { get; set; } = false;

    }
}
