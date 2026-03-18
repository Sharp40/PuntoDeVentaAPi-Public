using PuntoDeVenta.Domain.Constantes;

namespace PuntoDeVenta.API.Extensions;

public static class PoliciesConfiguration
{
    public static IServiceCollection AddPoliciesServices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.CREAR_USUARIO, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.CREAR_USUARIO)
                    )
                )
            );
        });


        // AGREGAR AQUÍ LAS DEMÁS POLICIES DE LA APLICACIÓN PARA PRODUCTOS
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.CREAR_PRODUCTO, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.CREAR_PRODUCTO)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.ACTUALIZAR_PRODUCTO, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.ACTUALIZAR_PRODUCTO)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.ACTUALIZAR_PRODUCTO, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.ACTUALIZAR_PRODUCTO)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.DESACTIVAR_PRODUCTO, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.DESACTIVAR_PRODUCTO)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.GENERAR_REPORTE_PRODUCTOS_BAJOS_EN_STOCK, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES 
                        && c.Value.Contains(Policy.GENERAR_REPORTE_PRODUCTOS_BAJOS_EN_STOCK)
                    )
                )
            );
        });


        // PROVEEDORES
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.CREAR_PROVEEDOR, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.CREAR_PROVEEDOR)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.ACTUALIZAR_PROVEEDOR, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.ACTUALIZAR_PROVEEDOR)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.LISTAR_PROVEEDORES, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.LISTAR_PROVEEDORES)
                    )
                )
            );
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policy.OBTENER_PROVEEDOR, policy => policy.RequireAssertion(
                    context => context.User.HasClaim(
                        c => c.Type == CustomClaims.POLICIES && c.Value.Contains(Policy.OBTENER_PROVEEDOR)
                    )
                )
            );
        });

        return services;
    }
}
