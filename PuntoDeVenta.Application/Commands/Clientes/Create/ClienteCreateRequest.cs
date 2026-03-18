using System.ComponentModel.DataAnnotations.Schema;

namespace PuntoDeVenta.Application.Commands.Clientes.Create;

public class ClienteCreateRequest
{
    public string? Nombre { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
}
