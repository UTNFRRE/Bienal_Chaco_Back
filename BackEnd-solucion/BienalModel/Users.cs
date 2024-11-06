using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Entidades
{
    public class MyUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        //navegabilidad para relacion muchos a muchos con votos
        [JsonIgnore]
        public ICollection<Votos>? Votos { get; set; }
    }
}
