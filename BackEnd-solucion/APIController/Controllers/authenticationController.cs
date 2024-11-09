using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Entidades;
using Microsoft.AspNetCore.Mvc;
using Requests;
using Servicios;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace APIBienal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioCreateRequest request)
        {
            var result = await _authenticationService.RegisterUserAsync(request);
            if (result)
            {
                return Ok("User registered successfully");
            }
            return BadRequest("User registration failed");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var access__token = await _authenticationService.LoginUserAsync(request);
            if (access__token != null)
            {
                return Ok(new { access__token });
            }
            return Unauthorized("Invalid login attempt");
        }
    }
}