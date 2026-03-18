using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Domain.Entidades.Otro;
using PuntoDeVenta.Domain.Interfaces;

namespace PuntoDeVenta.API.Controllers;

[Route("api/ventas")]
[ApiController]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly CrearVentaCommand _crearVentaCommand;
    private readonly IGenerarTicketService _generarTicketService;

    public VentasController(CrearVentaCommand crearVentaCommand, IGenerarTicketService generarTicketService)
    {
        _crearVentaCommand = crearVentaCommand;
        _generarTicketService = generarTicketService;
    }

    [Authorize]
    [HttpPost("crearConTicket")]
    public async Task<IActionResult> CrearVentaYGenerarTicket([FromBody] VentaCreateRequest request)
    {
        try
        {
            // Ejecutar comando para crear venta
            var venta = await _crearVentaCommand.EjecutarAsync(
                request.Cliente!,
                request.MetodoPago!,
                request.TotalVenta,
                request.MontoRecibido,
                request.Cambio,
                request.Detalles!
            );

            // Preparar datos para el ticket PDF
            var ticketInfo = new TicketInfo
            {
                Fecha = venta.Fecha,
                Cajero = venta.UsuarioNombreCompleto,
                Cliente = venta.ClienteNombre,
                Telefono = venta.ClienteTelefono,
                IdVenta = venta.IdVenta,
                MetodoPago = venta.MetodoPago,
                MontoRecibido = venta.MontoRecibido
            };

            var ticketDetalles = venta.Detalles.Select(d => new TicketDetalle
            {
                Producto = d.ProductoNombre,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario,
                Importe = d.Importe,
                Presentacion = null // o usa un campo si tienes
            }).ToList();

            // Generar el PDF
            var stream = await _generarTicketService.GenerarTicket(ticketDetalles, ticketInfo);

            // Retornar archivo PDF como resultado HTTP
            return File(stream, "application/pdf", $"ticket_{venta.IdVenta}.pdf");
        }
        catch (Exception ex)
        {
            // Manejo simple de error, puede mejorarse
            return BadRequest(new { error = ex.Message });
        }
    }
}

// Modelo para request (puede estar en otro archivo)
public class VentaCreateRequest
{
    public Cliente? Cliente { get; set; }
    public string? MetodoPago { get; set; }
    public decimal TotalVenta { get; set; }
    public decimal MontoRecibido { get; set; }
    public decimal Cambio { get; set; }
    public List<DetalleVentaInput>? Detalles { get; set; }
}


