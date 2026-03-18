using PuntoDeVenta.Domain.Entidades;

namespace PuntoDeVenta.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(Usuario user);
    Task<string> RefreshToken(Guid userId);
}
