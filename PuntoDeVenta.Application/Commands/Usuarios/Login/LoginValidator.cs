using FluentValidation;

namespace PuntoDeVenta.Application.Commands.Usuarios.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("El usuario es requerido.")
            //Agregar validaciones despues para el formato example@dominio.com
            .NotNull().WithMessage("El usuario es requerido.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es requerida.")
            .NotNull().WithMessage("La contraseña es requerida.");
    }
}
