using FluentValidation;

namespace PuntoDeVenta.Application.Commands.DetalleVentas.Create;

public class DetalleVentaCreateValidator : AbstractValidator<DetalleVentaCreateRequest>
{
    public DetalleVentaCreateValidator()
    {
        RuleFor(dV => dV.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor que cero.");

        RuleFor(dV => dV.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.");
    }
}
