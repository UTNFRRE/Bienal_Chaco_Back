using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Requests.Identity
{
    public class UserRolDTO
    {
        public string UserName { get; set; }
        public string? RoleName { get; set; }

        public UserRolDTO(string user, string rolName)
        {
            UserName = user;
            RoleName = rolName;
        }
    }
    


}
