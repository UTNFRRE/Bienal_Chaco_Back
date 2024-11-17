using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Requests;
using Microsoft.AspNetCore.Identity; // Asegúrate de importar esto para UserManager
using Contexts;
using Requests.Identity;
using System.Security.Claims;



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

        public async Task<UserInfoDTO> GetUserInfoByEmailAsync(string email)
        {
            var userLogueado = await _userManager.FindByEmailAsync(email);

            if (userLogueado == null)
            {
                throw new Exception("No se encontró ningún usuario con el correo electrónico proporcionado.");
            }

            var id = userLogueado.Id;
            var roles = (await _userManager.GetRolesAsync(userLogueado)).FirstOrDefault();

            var userInfo = new UserInfoDTO(id, roles);

            return userInfo;

        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task DeleteUserAsync(string email)
        {
            var userToDelete = await _userManager.FindByEmailAsync(email);
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
        Task<UserInfoDTO> GetUserInfoByEmailAsync(string email);
        Task Logout();
        Task DeleteUserAsync(string email);

    }
    
}

