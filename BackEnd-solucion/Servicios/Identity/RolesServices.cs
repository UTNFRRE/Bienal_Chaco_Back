using Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios.Identity
{
     public class RolesServices : IRolesServices
    {
        private readonly RoleManager<MyRol> _roleManager;
        private readonly UserManager<MyRol> _userManager;

        public RolesServices(RoleManager<MyRol> roleManager, UserManager<MyRol> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task CreateRoleAsync(MyRol role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception("Error al crear el rol: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        //obtener lista de roles
        public async Task<List<MyRol>>GetRolesAsync()
        {
            var myRoles = _roleManager.Roles.ToListAsync();
            return await myRoles;
                
        }   
    
        // metodo para buscar rol, pensado que puede ser admin o public
        public async Task<IdentityRole> GetRoleByName(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        //asignar rol a usuario
        public async Task AssignRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

           await _userManager.AddToRoleAsync(user, roleName);
          }


        public async Task DeleteRoleAsync(string name)
            {
            var roleToDelete = await _roleManager.FindByNameAsync(name);
            
            if (roleToDelete != null)
            {
                var result = await _roleManager.DeleteAsync(roleToDelete);
                if (!result.Succeeded)
                {
                    throw new Exception("Error al eliminar el rol: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}

    public interface IRolesServices
{
    Task CreateRoleAsync(MyRol role);
    Task<List<MyRol>> GetRolesAsync();
    Task<IdentityRole> GetRoleByName(string name);
    Task AssignRoleAsync(string email, string roleName);
    Task DeleteRoleAsync(string name);
}
