using FluentValidation;

namespace PuntoDeVenta.Application.Commands.RegistroCajas.Create;

public class RegistroCajaCreateValidator : AbstractValidator<RegistroCajaCreateRequest>
{
    public RegistroCajaCreateValidator()
    {
        RuleFor(r => r.FechaHora)
            .NotEmpty().WithMessage("La fecha y hora son obligatorias.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha y hora no pueden ser futuras.");

        RuleFor(r => r.MetodoPago)
            .MaximumLength(20).WithMessage("El método de pago no puede exceder los 20 caracteres.");

        RuleFor(r => r.MontoPagado)
            .GreaterThanOrEqualTo(0).WithMessage("El monto pagado debe ser mayor o igual a 0.");
    }
}
