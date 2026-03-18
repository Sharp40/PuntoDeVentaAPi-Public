namespace PuntoDeVenta.Application.Queries.Productos;

public record class ProductoResponse
{
    public Guid? Id { get; set; }
    public string? Nombre { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Stock { get; set; }
    public string? CodigoBarras { get; set; }
    public string? Descripcion { get; set; }
    public string? Proveedor { get; set; }
    public decimal PrecioVenta { get; set; }
    public string? Estado { get; set; }
    public int StockMinimo { get; set; }
}
