using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Servicios
{
    public class TokenService: ITokenService
    {
        private const string SecretKey = "Desarrollodesoftware2024&grupo9backyardMonsters"; // Cambia esto a una clave segura
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        public string GenerateToken(int esculturaId)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
            new Claim("esculturaId", esculturaId.ToString()), // Asociar el token a una escultura
            new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(now.AddMinutes(1)).ToUnixTimeSeconds().ToString()) // Expira en 1 minuto
        };

            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(1),
                signingCredentials: new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
    public interface ITokenService
    {
        string GenerateToken(int esculturaId);
    }
}
