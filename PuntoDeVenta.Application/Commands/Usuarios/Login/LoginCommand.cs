using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Interfaces;
using PuntoDeVenta.Application.Utils;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Usuarios.Login;

public class LoginCommand 
{
    public record LoginCommandRequest(LoginRequest LoginRequest) : IRequest<Result<LoginResponse>>, ICommandBase;

    internal class LoginCommandHandler : IRequestHandler<LoginCommandRequest, Result<LoginResponse>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly ITokenService _tokenService;
        public LoginCommandHandler(PuntoDeVentaDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var loginRequest = request.LoginRequest;

            // Buscar usuario por correo/usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreDeUsuario == loginRequest.Usuario, cancellationToken);

            if (usuario == null)
            {
                return Result<LoginResponse>.Failure("Usuario o contraseña incorrectos.");

            }

            var passwordHashIngresado = PasswordUtils.EncriptarPassword(loginRequest.Contrasena ?? "");

            if (usuario.Contrasena != passwordHashIngresado)
            {
                return Result<LoginResponse>.Failure("Usuario o contraseña incorrectos.");
            }

            // Construir perfil a retornar
            var loginResponse = new LoginResponse
            {
                Token = await _tokenService.CreateToken(usuario)
            };

            return Result<LoginResponse>.Success(loginResponse);
        }
    }
}
