namespace PuntoDeVenta.Application.Commands.Usuarios.Create;

public class UsuarioCreateRequest
{
    public required string Nombre { get; set; }
    public required string ApellidoPaterno { get; set; }
    public required string ApellidoMaterno { get; set; }
    public required string Usuario { get; set; }
    public required string Contrasena { get; set; }
    public required Rol Rol { get; set; } = Rol.Cajero;
}

public enum Rol
{
    Administrador,
    Cajero
}
