using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Requests;
using Microsoft.AspNetCore.Identity; // Asegúrate de importar esto para UserManager
using Contexts;
using Requests.Identity;



namespace Servicios
{
    public class UsersServices : IServiceUsers
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        public UsersServices(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
    

        public async Task<IEnumerable<UserRolDTO>> GetAllUsersAsync()
        {
            var listaUsuarios = await _userManager.Users.ToListAsync();
            var listaUserRol = new List<UserRolDTO>();
            
            foreach (var user in listaUsuarios)
            {
                var usuario = user.UserName;
                var roles = await _userManager.GetRolesAsync(user);
                var rol = roles.FirstOrDefault();
                
                var userRol = new UserRolDTO(usuario, rol);
                listaUserRol.Add(userRol);
            }
            return listaUserRol;
        }

        public async Task<MyUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task Logout()
        {
           await _signInManager.SignOutAsync();
        }

        public async Task DeleteUserAsync(string id)
        {
            var userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null)
            {
                var result = await _userManager.DeleteAsync(userToDelete);
                if (!result.Succeeded)
                {
                    throw new Exception("Error al eliminar el usuario: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }

}

    public interface IServiceUsers
    {
        Task<IEnumerable<UserRolDTO>> GetAllUsersAsync();
        Task<MyUser> GetUserByIdAsync(string id);
        Task Logout();
       Task DeleteUserAsync(string id);

    }
    
}

