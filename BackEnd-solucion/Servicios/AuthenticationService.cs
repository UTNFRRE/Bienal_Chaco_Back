using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Requests;
using Contexts;
using Models; 

using Entidades;
using Microsoft.AspNetCore.Identity;
using Requests;
using System.Threading.Tasks;
using Azure;



namespace Servicios
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<MyUser> _userManager;
        private readonly SignInManager<MyUser> _signInManager;

        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticationService(UserManager<MyUser> userManager, SignInManager<MyUser> signInManager, IJwtTokenService jwtTokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<bool> RegisterUserAsync(UsuarioCreateRequest request)
        {
            var user = new MyUser
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            return result.Succeeded;
        }

        public async Task<string> LoginUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, true, true);
            if (result.Succeeded)
            {
                var acces__token = _jwtTokenService.CreateToken(user);
                return acces__token;
            }
            return "Invalid login attempt";
        }
    }

    public interface IAuthenticationService
    {
        Task<bool> RegisterUserAsync(UsuarioCreateRequest request);
        Task<string> LoginUserAsync(LoginRequest request);
    }

}