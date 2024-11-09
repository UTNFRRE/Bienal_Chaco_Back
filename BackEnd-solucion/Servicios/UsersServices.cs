using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Requests;
using Microsoft.AspNetCore.Identity; // Asegúrate de importar esto para UserManager
using Contexts;



namespace Servicios
{
    public class UsersServices : IServiceUsers
    {
        private readonly UserManager<MyUser> _userManager;

        public UsersServices(UserManager<MyUser> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }
    

        public async Task<IEnumerable<MyUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<MyUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
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
    Task<IEnumerable<MyUser>> GetAllUsersAsync();
    Task<MyUser> GetUserByIdAsync(string id);
    Task DeleteUserAsync(string id);

    }
    
}

