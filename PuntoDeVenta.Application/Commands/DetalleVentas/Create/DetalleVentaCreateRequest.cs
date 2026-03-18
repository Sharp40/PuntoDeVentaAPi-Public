namespace PuntoDeVenta.Application.Commands.DetalleVentas.Create;

public class DetalleVentaCreateRequest
{
    public Guid VentaId { get; set; }
    public Guid ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; } // Es el precio de venta del producto, pero solo uno
}
