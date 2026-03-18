using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Productos.Create;

public class ProductoCreateValidator : AbstractValidator<ProductoCreateRequest>
{
    private readonly PuntoDeVentaDbContext _context;
    public ProductoCreateValidator(PuntoDeVentaDbContext context)
    {
        _context = context;

        RuleFor(p => p.Nombre)
            .NotNull().WithMessage("El nombre no puede ser nulo.")
            .NotEmpty().WithMessage("El nombre no puede estar vacío.")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres.")
            .Matches(@"^[a-zA-Z0-9\sáéíóúÁÉÍÓÚñÑ.,-]*$").WithMessage("El nombre contiene caracteres no válidos.");

        RuleFor(p => p.PrecioUnitario)
            .NotNull().WithMessage("El precio unitario no puede ser nulo.")
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor que cero.")
            .LessThanOrEqualTo(9999999999.99m).WithMessage("El precio unitario es demasiado grande.")
            .Must(p => DecimalPlaces(p) <= 2).WithMessage("El precio unitario debe tener máximo 2 decimales.");

        RuleFor(p => p.Stock)
            .NotNull().WithMessage("El stock no puede ser nulo.")
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo.")
            .LessThanOrEqualTo(1000000).WithMessage("El stock es demasiado grande.");

        RuleFor(p => p.CodigoBarras)
            .NotEmpty().WithMessage("El código de barras es obligatorio.")
            .Length(8, 50).WithMessage("El código de barras debe tener entre 8 y 50 caracteres.");

        RuleFor(p => p.Descripcion)
            .NotNull().WithMessage("La descripción no puede ser nula.")
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(50).WithMessage("La descripción no debe de exeder de los 50 caracteres.");

        RuleFor(p => p.PrecioVenta)
            .NotNull().WithMessage("El precio de venta no puede ser nulo.")
            .GreaterThan(0).WithMessage("El precio de venta debe ser mayor que cero.")
            .LessThanOrEqualTo(9999999999.99m).WithMessage("El precio de venta es demasiado grande.")
            .Must(p => DecimalPlaces(p) <= 2).WithMessage("El precio de venta debe tener máximo 2 decimales.");

        RuleFor(p => p.ProveedorId)
            .NotNull().WithMessage("El proveedor no puede ser nulo.")
            .MustAsync(async (proveedorId, cancellation) =>
            {
                return await _context.Proveedores.AnyAsync(p => p.Id == proveedorId, cancellation);
            })
            .WithMessage("El proveedor no existe.");
    }

    private int DecimalPlaces(decimal number)
    {
        number = Math.Abs(number); // Por si es negativo (aunque validas > 0)
        number -= Math.Truncate(number);
        int decimalPlaces = 0;
        while (number > 0)
        {
            number *= 10;
            number -= Math.Truncate(number);
            decimalPlaces++;
            if (decimalPlaces > 2) break;
        }
        return decimalPlaces;
    }
}
