using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Requests
{
    public class UsuarioCreateRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UsuarioUpdateRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UsuarioPatchRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}


