using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Interfaces;
using PuntoDeVenta.Application.Utils;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Usuarios.Create;

public class UsuarioCreateCommand
{
    public record UsuarioCreateCommandRequest(UsuarioCreateRequest UsuarioCreateRequest)
        : IRequest<Result<LoginResponse>>, ICommandBase;

    internal class UsuarioCreateCommandHandler : IRequestHandler<UsuarioCreateCommandRequest, Result<LoginResponse>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly ITokenService _tokenService;

        public UsuarioCreateCommandHandler(PuntoDeVentaDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(UsuarioCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var usuarioId = Guid.NewGuid();

            var usuario = new Usuario
            {
                Id = usuarioId,
                Nombre = request.UsuarioCreateRequest.Nombre,
                ApellidoPaterno = request.UsuarioCreateRequest.ApellidoPaterno,
                ApellidoMaterno = request.UsuarioCreateRequest.ApellidoMaterno,
                NombreDeUsuario = request.UsuarioCreateRequest.Usuario,
                Contrasena = PasswordUtils.EncriptarPassword(request.UsuarioCreateRequest.Contrasena),
                Rol = request.UsuarioCreateRequest.Rol.ToString()
            };

            _context.Add(usuario);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (result)
            {
                var loginResponse = new LoginResponse
                {
                    Token = await _tokenService.CreateToken(usuario)
                };
                return Result<LoginResponse>.Success(loginResponse);
            }

            return Result<LoginResponse>.Failure("Ups!, Por ahora no se pudo agregar el nuevo usuario, intentelo más tarde.");
        }
    }

    public class UsuarioCreateCommandRequestValidator : AbstractValidator<UsuarioCreateCommandRequest>
    {
        public UsuarioCreateCommandRequestValidator(PuntoDeVentaDbContext context)
        {
            RuleFor(x => x.UsuarioCreateRequest).SetValidator(new UsuarioCreateValidator(context));
        }
    }
}
