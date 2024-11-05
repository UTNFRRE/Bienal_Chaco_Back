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
    public class UsersServices : ICRUDServiceUsers
    {
        private readonly BienalDbContext _context;
        private readonly UserManager<MyUser> _userManager; // Añadido para manejar la identidad

        public UsersServices(BienalDbContext context, UserManager<MyUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Inicialización del UserManager
        }

        // Crear un nuevo Usuario
        public async Task<MyUser> CreateUserAsync(UsuarioCreateRequest request)
        {
            var newUser = new MyUser
            {
                UserName = request.Username, // Asigna los valores del request al nuevo usuario
                Email = request.Email,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth
                // Puedes agregar más propiedades según tu modelo
            };

            var result = await _userManager.CreateAsync(newUser, request.Password); // Crea el usuario con la contraseña
            if (!result.Succeeded)
            {
                throw new Exception("Error al crear el usuario: " + string.Join(", ", result.Errors));
            }

            return newUser;
        }

        // Obtener todos los Users
        public async Task<IEnumerable<MyUser>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Obtener un User por su ID
        public async Task<MyUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        // Actualizar un User existente
        public async Task<MyUser> UpdateUserAsync(int id, UsuarioUpdateRequest request)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser != null)
            {
                // Actualiza las propiedades necesarias
                existingUser.Email = request.Email;
                existingUser.FullName = request.FullName;
                existingUser.DateOfBirth = request.DateOfBirth;
                // Otras actualizaciones...

                await _context.SaveChangesAsync();
            }
            return existingUser;
        }

        public async Task<MyUser> UpdatePatchUserAsync(int id, UsuarioPatchRequest request)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser != null)
            {
                // Actualiza las propiedades necesarias
                if (request.Email != null)
                {
                    existingUser.Email = request.Email;
                }
                if (request.FullName != null)
                {
                    existingUser.FullName = request.FullName;
                }
                if (request.DateOfBirth != null)
                {
                    existingUser.DateOfBirth = request.DateOfBirth;
                }
                // Otras actualizaciones...

                await _context.SaveChangesAsync();
            }
            return existingUser;
        }

        // Eliminar un User
        public async Task DeleteUserAsync(int id)
        {
            var userToDelete = await _context.Users.FindAsync(id);
            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }
        }

    }

    public interface ICRUDServiceUsers
    {
        Task<MyUser> CreateUserAsync(UsuarioCreateRequest request);
        Task<IEnumerable<MyUser>> GetAllUsersAsync();
        Task<MyUser> GetUserByIdAsync(int id);
        Task<MyUser> UpdateUserAsync(int id, UsuarioUpdateRequest request);
        Task<MyUser> UpdatePatchUserAsync(int id, UsuarioPatchRequest request);
        Task DeleteUserAsync(int id);
    }
}