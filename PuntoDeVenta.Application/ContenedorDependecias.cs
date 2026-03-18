using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PuntoDeVenta.Application.Core;

namespace PuntoDeVenta.Application;

public static class ContenedorDependecias
{
    public static IServiceCollection AddDependenciasApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration => 
        {
            configuration.RegisterServicesFromAssembly(typeof(ContenedorDependecias).Assembly);

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ContenedorDependecias).Assembly);

        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped<CrearVentaCommand>();


        return services;
    }
}
