using MediatR;
using PuntoDeVenta.Domain.Entidades.Otro;
using PuntoDeVenta.Domain.Interfaces;
using PuntoDeVenta.Infrastructure.Reports;

namespace PuntoDeVenta.Application.Queries.Ventas.GetTicket;

public class GenerarTicket
{
    public record GenerarTicketQuery(GenerarTicketRequest Request) : IRequest<MemoryStream>;

    internal class GenerarTicketHandler : IRequestHandler<GenerarTicketQuery, MemoryStream>
    {
        private readonly IGenerarTicketService _ticketService;

        public GenerarTicketHandler(IGenerarTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public async Task<MemoryStream> Handle(GenerarTicketQuery request, CancellationToken cancellationToken)
        {
            var info = new TicketInfo
            {
                Fecha = request.Request.Fecha,
                Cajero = request.Request.Cajero!,
                Cliente = request.Request.Cliente!,
                Telefono = request.Request.Telefono!,
                IdVenta = request.Request.IdVenta,
                MetodoPago = request.Request.MetodoPago!,
                MontoRecibido = request.Request.MontoRecibido
            };

            var detalles = request.Request.Productos.Select(p => new TicketDetalle
            {
                Producto = p.Producto,
                Presentacion = p.Presentacion,
                Cantidad = p.Cantidad,
                PrecioUnitario = p.PrecioUnitario,
                Importe = p.Importe
            }).ToList();

            return await _ticketService.GenerarTicket(detalles, info);
        }
    }

}

