namespace PuntoDeVenta.Application.Queries.Productos.GetProductosBajosEnStock;

public class ProductosBajosEnStockResponse
{
    public Guid Id { get; set; }
    public string? Nombre { get; set; }
    public string? CodigoDeBarras { get; set; }
    public string? Descripcion { get; set; }
    public int Stock { get; set; }
    public DateTime? FechaAlerta { get; set; }
    public string? Mensaje { get; set; }
    public string? Estado { get; set; }
}
