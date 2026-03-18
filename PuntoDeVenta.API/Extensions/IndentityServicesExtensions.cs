using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PuntoDeVenta.Application.Interfaces;
using PuntoDeVenta.Domain.Interfaces;
using PuntoDeVenta.Infrastructure.Security;

namespace PuntoDeVenta.API.Extensions;

public static class IndentityServicesExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services, IConfiguration configuration)
    {

        // Inyección del token

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserAccessor, UserAccesor>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]!));

        // validar por signed key
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,

                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role,
                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.Zero,

                };
            }
        );

        return services;
    }
}
