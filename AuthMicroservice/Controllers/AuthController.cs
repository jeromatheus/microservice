using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto login)
        {
            // 1. Validar usuario (Simulación de Base de Datos)
            if (login.UserName == "admin" && login.Password == "1234")
            {
                var token = GenerateJwtToken(login.UserName);
                return Ok(new { token = token });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            // 2. Obtener la clave del appsettings
            var secretKey = _config["JwtSettings:SecretKey"];
            var keyBytes = Encoding.UTF8.GetBytes(secretKey);

            // 3. Crear los Claims (Datos del usuario dentro del token)
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, username));
            claims.AddClaim(new Claim(ClaimTypes.Role, "Admin")); // Ejemplo de rol

            // 4. Crear las credenciales de firma
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                // El token expira en 1 hora
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(tokenConfig);
        }
    }

    // DTO simple para recibir los datos
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}