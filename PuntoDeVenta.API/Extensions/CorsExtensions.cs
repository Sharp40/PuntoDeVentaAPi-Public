using PuntoDeVenta.Domain.Constantes;

namespace PuntoDeVenta.API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsExtension(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>();


        services.AddCors(options =>
        {
            options.AddPolicy(Policy.FRONTEND_POLICY, builder =>
            {
                builder.WithOrigins(origins!)
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials();
            });
        });

        return services;
    }
}
