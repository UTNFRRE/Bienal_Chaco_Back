using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Entidades;
using Servicios;
using Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;


namespace APIBienal.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {

        private readonly IServiceUsers _userService;

        public UsuariosController(IServiceUsers userService)
        {
            _userService = userService;
        }

        // Obtener todos los Usuarios
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var listadeUsuarios = await _userService.GetAllUsersAsync();
            return listadeUsuarios == null ? NotFound() : Ok(listadeUsuarios);
        }

        // Obtener un Usuario por email
        [HttpGet("InfoUsuario")]
        public async Task<IActionResult> GetUserInfoByEmailAsync(string email)
        {
            var usuarioObtenido = await _userService.GetUserInfoByEmailAsync(email);

            if (usuarioObtenido == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(usuarioObtenido);
            }
        }
        [Authorize(AuthenticationSchemes = "Identity.Bearer")]
        [HttpPost("Logout")]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _userService.Logout();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Eliminar un Usuario
        [Authorize(AuthenticationSchemes = "Identity.Bearer", Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return Forbid();
            }

            await _userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
