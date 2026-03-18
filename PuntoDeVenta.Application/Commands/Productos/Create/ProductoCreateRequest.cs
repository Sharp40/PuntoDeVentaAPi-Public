namespace PuntoDeVenta.Application.Commands.Productos.Create;

public class ProductoCreateRequest : ProductoRequest
{
    public Guid? ProveedorId { get; set; }
    public int Stock { get; set; }
}
