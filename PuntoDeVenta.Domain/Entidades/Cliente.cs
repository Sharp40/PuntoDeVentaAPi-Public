using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class Cliente
{
    [Column("id_cliente")]
    public Guid Id { get; set; }
    [Column("nombre")]
    public string? Nombre { get; set; }
    [Column("telefono")]
    public string? Telefono { get; set; }
    [Column("email")]
    public string? Email { get; set; }
    [Column("direccion")]
    public string? Direccion { get; set; }
}
