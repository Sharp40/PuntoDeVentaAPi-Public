using FluentValidation;
using PuntoDeVenta.Application.Utils;
using PuntoDeVenta.Application.Utils.CustomValidations;

namespace PuntoDeVenta.Application.Commands.Proveedores.Update;

public class ProveedorUpdateValidator : AbstractValidator<ProveedorUpdateRequest>
{
    public ProveedorUpdateValidator()
    {
        RuleFor(p => p.RazonSocial)
            .NotEmpty().WithMessage("El campo 'Razon social' no debe estar vacio.")
            .NotNull().WithMessage("El campo 'Razon social' no debe estar vacio.")
            .MaximumLength(50).WithMessage("La razon social no debe de exceder los 50 caracteres")
            .Must(ProveedorCustomValidation.IsValidRazonSocial!)
            .WithMessage("El campo 'Razon social' no tiene el formato correcto.");

        RuleFor(p => p.RFC)
            .NotEmpty().WithMessage("El campo 'RFC' no debe estar vacio.")
            .NotNull().WithMessage("El campo 'RFC' no debe estar vacio.")
            .MaximumLength(50).WithMessage("El RFC es demasiado largo")
            .Must(ProveedorCustomValidation.IsValidRFC!)
            .WithMessage("El campo 'RFC' no tiene el formato correcto.");

        RuleFor(p => p.Nombre)
            .NotEmpty().WithMessage("El campo 'Nombre' no debe estar vacio.")
            .NotNull().WithMessage("El campo 'Nombre' no debe estar vacio.")
            .MaximumLength(100).WithMessage("El nombre del proveedor debe tener como máximo 100 catacteres.")
            .Must(Validations.IsValidName!)
            .WithMessage("El 'Nombre' no es valido.");

        RuleFor(p => p.Telefono)
            .NotEmpty().WithMessage("El campo 'Telefono' no debe estar vacio.")
            .MaximumLength(20).WithMessage("El campo 'Telefono' no tiene el formato correcto.")
            .MinimumLength(10).WithMessage("El campo 'Telefono' no tiene el formato correcto.")
            .NotNull().WithMessage("El campo 'Telefono' no debe estar vacio.")
            .Must(Validations.IsValidPhoneNumber!)
            .WithMessage("El campo 'Telfono' no tiene el formato correcto");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("El campo 'Email' no debe estar vacio.")
            .NotNull().WithMessage("El campo 'Email' no debe estar vacio.")
            .MaximumLength(100).WithMessage("El correo introducido es demasaido largo.")
            .Must(Validations.IsValidEmail!)
            .WithMessage("El campo 'Email' no tiene el formato correcto");

        RuleFor(p => p.Direccion)
            .NotEmpty().WithMessage("El campo 'Dirección' no debe estar vacio.")
            .NotNull().WithMessage("El campo 'Dirección no debe estar vacio.")
            .MaximumLength(500).WithMessage("La direccion introducida es demasiado larga.")
            .Must(AddressCustomValidation.IsValidStreet!) // Agrega la logica de validar dirección.
            .WithMessage("El campo 'Dirección' no tiene el formato correcto");
    }
}
