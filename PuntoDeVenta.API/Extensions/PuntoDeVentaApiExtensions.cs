using PuntoDeVenta.Application;
using PuntoDeVenta.Domain.Constantes;
using PuntoDeVenta.Infrastructure;

namespace PuntoDeVenta.API.Extensions;

public static class PuntoDeVentaApiExtensions
{
    public static IServiceCollection AddExtensionesAPI(this IServiceCollection services, 
                                                            string cadenaConexion,
                                                            IConfiguration configuration)
    {


        services.AddControllers();
        services.AddOutputCache();

        // Agregar configuración de Swagger 
        services.AddCustomSwagger();

        //Otro
        services.AddHttpContextAccessor();
        services.AddIdentityServices(configuration);
        

        //Agregar configuración de CORS
        services.AddCorsExtension(configuration);

        // Agregar servicios de la capa de aplicación y persistencia
        services.AddDependenciasInfrastructure(cadenaConexion);
        services.AddDependenciasApplication();
        


        return services;
    }

    public static WebApplication UseExtensionesAPI(this WebApplication app)
    {
        // Configuración de Swagger
        // if (app.Environment.IsDevelopment())
        // {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.ConfigObject.AdditionalItems["withCredentials"] = true;
            });
        //}

        app.UseCors(Policy.FRONTEND_POLICY);
        app.UseMiddleware<Middleware.ExceptionMiddleware>();
        app.UseHttpsRedirection();
        

        app.UseAuthentication();
        app.UseMiddleware<Middleware.TokenBindingMiddleware>();
        app.UseAuthorization();
        app.UseOutputCache();

        
        app.MapControllers();

        return app;
    }
}
