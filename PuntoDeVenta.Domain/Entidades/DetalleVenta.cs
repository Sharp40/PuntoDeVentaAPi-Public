using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class DetalleVenta
{
    [Column("id_detalle_venta")]
    public Guid Id { get; set; }

    [Column("id_venta")]
    public Guid VentaId { get; set; }
    public Venta? Venta { get; set; }

    [Column("id_producto")]
    public Guid ProductoId { get; set; }
    public Producto? Producto { get; set; }

    [Column("cantidad")]
    public int Cantidad { get; set; }
    [Column("precio_unitario")]
    public decimal PrecioUnitario { get; set; }
}
