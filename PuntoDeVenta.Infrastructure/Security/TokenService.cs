using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PuntoDeVenta.Application.Interfaces;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Infrastructure.Security;

public class TokenService : ITokenService
{
    private readonly PuntoDeVentaDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenService(PuntoDeVentaDbContext context, 
                        IConfiguration configuration, 
                        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> CreateToken(Usuario user)
    {
        const int expireTime = 1; // Tiempo de expiracion en minutos

        var userRole = await _context.Usuarios
            .Where(u => u.NombreDeUsuario == user.NombreDeUsuario)
            .Select(user => user.Rol)
            .FirstOrDefaultAsync();

        var idJwt = Guid.NewGuid().ToString();

        // Generacion del fingerprint
        var fingerprint = Guid.NewGuid().ToString();
        byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(fingerprint));
        var fingerprintHash = Convert.ToBase64String(hashBytes);

       
        var isHttps = _httpContextAccessor.HttpContext?.Request.IsHttps ?? false;
        var sameSite = isHttps ? SameSiteMode.None : SameSiteMode.Lax;

        // Setear cookie
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = isHttps,
            SameSite = sameSite,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(expireTime)
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("Secure_Fingerprint", fingerprint, cookieOptions);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, userRole ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, idJwt),
            new Claim("fprint", fingerprintHash),

        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["TokenKey"]!)
            ),
            SecurityAlgorithms.HmacSha256Signature
        );

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            // Agui defines si el token expira en 1 dia o lo persoanlizas para horas, semanasm etc.
            Expires = DateTime.UtcNow.AddDays(expireTime),
            SigningCredentials = credentials

        };

        // Decodificar
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);  // Retorna el token encriptado
    }

    public Task<string> RefreshToken(Guid userId)
    {
        throw new NotImplementedException();
    }
}
