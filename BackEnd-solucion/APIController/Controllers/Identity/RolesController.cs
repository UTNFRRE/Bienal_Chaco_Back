using Entidades;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIController.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesServices rolesServices;

        public RolesController(IRolesServices _rolesservices)
        {
            rolesServices = _rolesservices;
        }

        [HttpPost("CreateRol")]
        public async Task<IActionResult> CreateRol(string nombreRol)
            {
             await rolesServices.CreateRoleAsync(nombreRol);
            return Ok();
        }

        [HttpGet("Lista de Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await rolesServices.GetRolesAsync();
            //validacion si result tiene un valor 
            if (result != null)
            {
                return Ok(result);
            } else
            {
                return NotFound();
            }
        }

        // PUT api/<RolesController>/5
        [HttpPost("AsignarRol")]
        public async Task<IActionResult> AsignateRol(string email, string rolename)
        {
            await rolesServices.AssignRoleAsync(email, rolename);
            return Ok();
        }

        // DELETE api/<RolesController>/5
        [HttpDelete]
        public async Task<IActionResult> DeleteRol(string nameRole)
        {
            await rolesServices.DeleteRoleAsync(nameRole);
            return Ok();
        }
    }
}
