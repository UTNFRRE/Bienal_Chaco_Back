using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Entidades;
using Servicios;
using Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt; // Añadido
using Microsoft.IdentityModel.Tokens; // Añadido
using System.Text; // Añadido


namespace APIBienal.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ICRUDServiceUsers _userService;
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UsuariosController(ICRUDServiceUsers userService, UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IConfiguration configuration)
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // Crear un nuevo Usuario
        [HttpPost("create")]
        public async Task<IActionResult> CreateUsuario([FromForm] UsuarioCreateRequest request)
        {
            var user = new MyUser { UserName = request.Email, Email = request.Email, FullName = request.FullName, DateOfBirth = request.DateOfBirth };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                return Ok(new { Result = "User created successfully" });
            }

            return BadRequest(result.Errors);
        }

        // Login de usuario
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            var result = await _signInManager.PasswordSignInAsync(request.Email, request.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                var token = GenerateJwtToken(user);

                // Enviar el token como cookie
                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                return Ok(new { Message = "Login exitoso" });
            }

            return Unauthorized(new { Message = "Credenciales incorrectas" });
        }

        private string GenerateJwtToken(MyUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Obtener todos los Usuarios
        [HttpGet]
        public async Task<IActionResult> GetAllUsuarios()
        {
            var listadeUsuarios = await _userService.GetAllUsersAsync();
            return listadeUsuarios == null ? NotFound() : Ok(listadeUsuarios);
        }

        // Obtener un Usuario por ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuarioById(int id)
        {
            var usuarioObtenido = await _userService.GetUserByIdAsync(id);

            if (usuarioObtenido == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(usuarioObtenido);
            }
        }

        // Actualizar un Usuario existente
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUsuario(int id, [FromBody] UsuarioUpdateRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return Forbid();
            }

            var usuarioActualizado = await _userService.UpdateUserAsync(id, request);
            return Ok(usuarioActualizado);
        }

        // Actualizar parcialmente un Usuario existente
        [Authorize]
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdatePatchUsuario(int id, [FromBody] UsuarioPatchRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != id.ToString())
            {
                return Forbid();
            }

            var usuarioActualizado = await _userService.UpdatePatchUserAsync(id, request);
        return Ok(usuarioActualizado);
        }

        // Eliminar un Usuario
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
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