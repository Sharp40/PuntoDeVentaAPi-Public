using PuntoDeVenta.Application.Core;

namespace PuntoDeVenta.Application.Queries.Usuarios.GetUsuarios;

public class GetUsuariosRequest : PagingParams
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Usuario { get; set; }
    public string? Rol { get; set; }
}
