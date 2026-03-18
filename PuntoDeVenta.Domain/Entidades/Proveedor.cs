using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class Proveedor
{
    [Column("id_proveedor")]
    public Guid Id { get; set; }

    [Column("empresa_RazonSocial")]
    public string? RazonSocial { get; set; }

    public string? RFC { get; set; }

    [Column("nombre_provedor")]
    public string? Nombre { get; set; }

    [Column("telefono")]
    public string? Telefono { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("direccion")]
    public string? Direccion { get; set; }

    [Column("FechadeRegistro")]
    public DateTime? FechaDeRegistro { get; set; }
}
