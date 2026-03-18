namespace PuntoDeVenta.Application.Commands.Productos;

public abstract class ProductoRequest
{
    public string? Nombre { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Descripcion { get; set; }
    public decimal PrecioVenta { get; set; }
}
