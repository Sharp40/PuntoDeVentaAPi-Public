using FluentValidation;
using PuntoDeVenta.Application.Utils;
using PuntoDeVenta.Application.Utils.CustomValidations;

namespace PuntoDeVenta.Application.Commands.Clientes.Create;

public class ClienteCreateValidator : AbstractValidator<ClienteCreateRequest>
{
    public ClienteCreateValidator()
    {
        RuleFor(c => c.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .NotNull().WithMessage("El nombre no puede ser nulo.")
            .MaximumLength(100).WithMessage("El nombre no puede tener más de 100 caracteres.")
            .Must(Validations.IsValidName).WithMessage("El nombre no es valido.");

        RuleFor(c => c.Telefono)
            .NotEmpty().WithMessage("El teléfono es obligatorio.")
            .NotNull().WithMessage("El teléfono no puede ser nulo.")
            .MaximumLength(20).WithMessage("El teléfono no puede tener más de 20 caracteres.")
            .Must(Validations.IsValidPhoneNumber).WithMessage("El teléfono no es valido.");

        RuleFor(c => c.Email)
            .MaximumLength(100).WithMessage("El email no puede tener más de 100 caracteres.")
            .Must(Validations.IsValidEmail).When(c => !string.IsNullOrWhiteSpace(c.Email)).WithMessage("El email no es valido.");

        RuleFor(c => c.Direccion)
            .MaximumLength(500).WithMessage("La dirección no puede tener más de 500 caracteres.")
            .Must(AddressCustomValidation.IsValidStreet).When(c => !string.IsNullOrWhiteSpace(c.Direccion)).WithMessage("La dirección no es valida.");
    }
}
