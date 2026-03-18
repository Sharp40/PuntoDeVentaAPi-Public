using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Utils;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Usuarios.Create;

public class UsuarioCreateValidator : AbstractValidator<UsuarioCreateRequest>
{
    private readonly PuntoDeVentaDbContext _context;
    public UsuarioCreateValidator(PuntoDeVentaDbContext context)
    {
        _context = context;

        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
            .Must(Validations.IsValidName).WithMessage("El nombre contiene caracteres inválidos o no cumple el formato.");

        RuleFor(x => x.ApellidoPaterno)
            .NotEmpty().WithMessage("El apellido paterno es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido paterno no puede exceder los 100 caracteres.")
            .Must(Validations.IsValidName).WithMessage("El apellido paterno contiene caracteres inválidos o no cumple el formato.");

        RuleFor(x => x.ApellidoMaterno)
            .NotEmpty().WithMessage("El apellido materno es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido materno no puede exceder los 100 caracteres.")
            .Must(Validations.IsValidName).WithMessage("El apellido materno contiene caracteres inválidos o no cumple el formato.");

        RuleFor(x => x.Usuario)
            .NotEmpty().WithMessage("El nombre de usuario es obligatorio.")
            .MustAsync(UsuarioNoExiste).WithMessage("El nombre de usuario ya está en uso.")
            .MaximumLength(50).WithMessage("El nombre de usuario no debe exceder los 50 caracteres.")
            .Must(Validations.IsValidEmail).WithMessage("El nombre de usuario debe ser un correo electrónico válido.");

        RuleFor(x => x.Contrasena)
            .NotEmpty().WithMessage("La contraseña es obligatoria.")
            .MaximumLength(255).WithMessage("La contraseña es demasiado extensa.")
            .Must(Validations.IsStrongPassword).WithMessage("La contraseña debe tener al menos 8 caracteres, una mayúscula, una minúscula, un número y un carácter especial.");

        RuleFor(x => x.Rol)
            .IsInEnum().WithMessage("El rol seleccionado no es válido.");
    }

    private async Task<bool> UsuarioNoExiste(string username, CancellationToken cancellationToken)
    {
        return !await _context.Usuarios.AnyAsync(u => u.NombreDeUsuario == username, cancellationToken);
    }
}
