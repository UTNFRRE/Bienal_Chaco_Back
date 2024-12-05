using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Servicios
{
    public class TokenService: ITokenService
    {
        private const string SecretKey = "Desarrollodesoftware2024&grupo9backyardMonsters"; 
        private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

        public string GenerateToken(int esculturaId)
        {
            var now = DateTime.UtcNow;
            var claims = new[]
            {
            new Claim("esculturaId", esculturaId.ToString()), // Asociamos a una escultura
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
        public bool ValidateToken(string token, int idEscultura)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateIssuer = false,  
                ValidateAudience = false, 
                ValidateLifetime = true, // Verificamos si el token está expirado
                ClockSkew = TimeSpan.Zero // Eliminamos la tolerancia por desfase de reloj
            };

            try
            {
                // Validamos el token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Confirmamos que el token es un JWT válido
                if (validatedToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false; 
                }

                // Extraemos el esculturaId del token
                var esculturaIdClaim = principal.FindFirst("esculturaId");
                if (esculturaIdClaim == null || !int.TryParse(esculturaIdClaim.Value, out var tokenEsculturaId))
                {
                    return false; // El claim esculturaId no existe o es inválido
                }

                // Comparamos el esculturaId del token con el proporcionado
                return tokenEsculturaId == idEscultura;
            }
            catch (SecurityTokenExpiredException)
            {
                // El token está expirado
                return false;
            }
            catch (Exception)
            {
                // Cualquier otra excepción se considera token inválido
                return false;
            }
        }

    }
    public interface ITokenService
    {
        string GenerateToken(int esculturaId);
        bool ValidateToken(string token, int idescultura);
    }
}
