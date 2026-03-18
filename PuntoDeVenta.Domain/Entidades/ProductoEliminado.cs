
using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Domain.Entidades;

public class ProductoEliminado
{
    [Column("id_eliminacion")]
    public Guid Id { get; set; }
    [Column("id_producto")]
    public Guid? ProductoId { get; set; }
    [Column("nombre")]
    public string? Nombre { get; set; }
    [Column("precio_unitario")]
    public decimal? PrecioUnitario { get; set; }
    [Column("precio_venta")]
    public decimal? PrecioVenta { get; set; }
    [Column("stock")]
    public int Stock { get; set; }
    [Column("codigo_barras")]
    public string? CodigoBarras { get; set; }
    [Column("id_proveedor")]
    public Guid? ProveedorId { get; set; }
    [Column("fecha_eliminacion")]
    public DateTime FechaEliminacion { get; set; }
    [Column("id_usuario_elimino")]
    public Guid IdUsuarioElimino { get; set; }
    [Column("motivo_eliminacion")]
    public string? MotivoEliminacion { get; set; }
    [Column("descripcion")]
    public string? Descripcion { get; set; }
    public DateTime Fecha { get; set; }
}
