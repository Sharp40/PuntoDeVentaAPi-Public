namespace PuntoDeVenta.Application.Queries.Usuarios;

public record class UsuarioResponse
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? ApellidoPaterno { get; set; }
    public string? ApellidoMaterno { get; set; }
    public string? Usuario { get; set; }
    public string? Rol { get; set; }

}

