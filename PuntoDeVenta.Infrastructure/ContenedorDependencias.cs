using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntoDeVenta.Domain.Interfaces;
using PuntoDeVenta.Infrastructure.Persistence;
using PuntoDeVenta.Infrastructure.Reports;

namespace PuntoDeVenta.Infrastructure;

public static class ContenedorDependencias
{
    public static IServiceCollection AddDependenciasInfrastructure(this IServiceCollection services, string cadenaConexion)
    {
        services.AddDbContext<PuntoDeVentaDbContext>(options =>
            options.UseSqlServer(cadenaConexion, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            })
        );

        services.AddTransient(typeof(IReportService<>), typeof(ReportService<>));
        services.AddTransient<IGenerarTicketService, GenerarTicketService>();


        return services;
    }
}
