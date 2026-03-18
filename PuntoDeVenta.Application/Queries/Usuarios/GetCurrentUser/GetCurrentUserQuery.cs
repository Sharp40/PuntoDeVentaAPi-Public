using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Commands.Usuarios.Login;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Application.Interfaces;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Usuarios.GetCurrentUser;

public class GetCurrentUserQuery
{
    public record GetCurrentUserQueryRequest(GetCurrentUserRequest GetCurrentUserRequest) 
        : IRequest<Result<Profile>>;

    internal class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQueryRequest, Result<Profile>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly ITokenService _tokenService;
        public GetCurrentUserHandler(PuntoDeVentaDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Result<Profile>> Handle(GetCurrentUserQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _context
                .Usuarios
                .FirstOrDefaultAsync(user => user.Id == request.GetCurrentUserRequest.UserId);

            if (user is null)
                return Result<Profile>.Failure("No se encontro el usuario");

            var profile = new Profile
            {
                Nombre = user.Nombre,
                ApellidoPaterno = user.ApellidoPaterno,
                ApellidoMaterno = user.ApellidoMaterno,
                Usuario = user.NombreDeUsuario,
                Rol = user.Rol?.ToLowerInvariant(),
            };


            return Result<Profile>.Success(profile);
        }
    }
}
