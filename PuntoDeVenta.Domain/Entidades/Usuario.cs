using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class Usuario
{
    [Column("id_usuario")]
    public Guid Id { get; set; }

    [Column("nombre")]
    public string? Nombre { get; set; }

    [Column("usuario")]
    public string? NombreDeUsuario { get; set; }

    [Column("contrasena")]
    public string? Contrasena { get; set; }

    [Column("rol")]
    public string? Rol { get; set; }

    [Column("Apellido_Paterno")]
    public string? ApellidoPaterno { get; set; }

    [Column("Apellido_Materno")]
    public string? ApellidoMaterno { get; set; }
}
