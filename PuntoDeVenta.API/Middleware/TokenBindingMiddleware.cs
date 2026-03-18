using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PuntoDeVenta.API.Middleware;

public class TokenBindingMiddleware
{
    private readonly RequestDelegate _next;

    public TokenBindingMiddleware(RequestDelegate next) => _next = next; //COnstructor

    // En TokenBindingMiddleware.cs
    public async Task InvokeAsync(HttpContext context)
    {
        

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var hashInToken = context.User.FindFirstValue("fprint");

            if (!context.Request.Cookies.TryGetValue("Secure_Fingerprint", out var fingerprintRaw) || string.IsNullOrEmpty(hashInToken))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // 2. Generar el hash de la cookie recibida (usando el método estático más simple)
            var cleanFingerprint = fingerprintRaw.Replace("\"", "").Trim();
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(cleanFingerprint));
            var hashedCookie = Convert.ToBase64String(hashBytes);


            if (hashInToken != hashedCookie)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

        }

        await _next(context);
    }
}
