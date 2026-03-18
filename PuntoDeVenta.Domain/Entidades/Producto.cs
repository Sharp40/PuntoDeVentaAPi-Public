using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class Producto
{
    [Column("id_producto")]
    public Guid? Id { get; set; }

    [Column("nombre")]
    public string? Nombre { get; set; }

    [Column("precio_unitario")]
    public decimal? PrecioUnitario { get; set; }

    [Column("stock")]
    public int? Stock { get; set; }

    [Column("codigo_barras")]
    public string? CodigoBarras { get; set; }

    [Column("descripcion")]
    public string? Descripcion { get; set; }


    [Column("id_proveedor")]
    public Guid? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }


    [Column("precio_venta")]
    public decimal? PrecioVenta { get; set; }
    public string? Estado { get; set; }

    [Column("stock_minimo")]
    public int? StockMinimo { get; set; }
}
