namespace PuntoDeVenta.Application.Commands.Productos.Update;

public class ProductoUpdateRequest : ProductoRequest
{
    public int AgregarStock { get; set; }
}
