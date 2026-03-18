using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class ProductosBajosEnStock
{
    [Column("id_producto")]
    public Guid Id { get; set; }
    [Column("nombre")]
    public string? Nombre { get; set; }
    [Column("codigo_barras")]
    public string? CodigoDeBarras { get; set; }
    [Column("descripcion")]
    public string? Descripcion { get; set; }
    [Column("stock")]
    public int? Stock { get; set; }
    public DateTime? FechaAlerta { get; set; }
    public string? Mensaje { get; set; }
    public string? Estado { get; set; }
}
