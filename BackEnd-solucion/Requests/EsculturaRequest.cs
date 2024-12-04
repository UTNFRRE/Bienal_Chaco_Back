using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Entidades;

namespace Requests
{
    public class EsculturaPostRequest
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = "";
        [MaxLength(200)]
        public string? Descripcion { get; set; }

        [Required]
        public IFormFile[] Imagenes { get; set; } // Archivos de imágenes

        //primero sin control de que exista ese escultor
        [Required]
        public int EscultorID { get; set; }

        //desde aca va el GetAll
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public string? Tematica { get; set; }
        [Required]
        public int EdicionAño { get; set; }
    }

    public class EsculturaPutRequest
    {
        [MaxLength(50)]
        public string Nombre { get; set; } = "";
        [MaxLength(200)]
        public string? Descripcion { get; set; }
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public int EscultorID { get; set; }
        public string? Tematica { get; set; }
        public int EdicionAño { get; set; }
        public IFormFile[] NuevasImagenes { get; set; } // Nuevas imágenes a subir
        public int[] ImagenesAEliminar { get; set; } // IDs de imágenes a eliminar (opcional)
    }


    public class EsculturaPatch
    {  
        [MaxLength(50)]
        public string? Nombre { get; set; }
        [MaxLength(200)]
        public string? Descripcion { get; set; }

        public List<Imagen> Imagenes { get; set; }

        public int? EscultorID { get; set; }
        public DateOnly? FechaCreacion { get; set; }
        public string? Tematica { get; set; }
    }

    public class EsculturaVoto
    {
        public int Voto { get; set; }
    }
}
