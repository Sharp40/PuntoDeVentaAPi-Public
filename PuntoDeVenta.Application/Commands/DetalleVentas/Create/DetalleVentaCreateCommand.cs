using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.DetalleVentas.Create;

public class DetalleVentaCreateCommand
{
    public record DetalleVentaCreateCommandRequest(DetalleVentaCreateRequest DetalleVentaCreateRequest)
        : IRequest<Result<Guid>>;

    internal class DetalleVentaCreateCommandHandler
        : IRequestHandler<DetalleVentaCreateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public DetalleVentaCreateCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(DetalleVentaCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var detalleVentaId = Guid.NewGuid();

            var detalleVenta = new Domain.Entidades.DetalleVenta
            {
                Id = detalleVentaId,
                VentaId = request.DetalleVentaCreateRequest.VentaId,
                ProductoId = request.DetalleVentaCreateRequest.ProductoId,
                Cantidad = request.DetalleVentaCreateRequest.Cantidad,
                PrecioUnitario = request.DetalleVentaCreateRequest.PrecioUnitario
            };

            _context.Add(detalleVenta);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(detalleVentaId)
                : Result<Guid>.Failure("Error al crear el detalle de la venta");
        }
    }

    public class DetalleVentaCreateCommandRequestValidator : AbstractValidator<DetalleVentaCreateCommandRequest>
    {
        public DetalleVentaCreateCommandRequestValidator()
        {
            RuleFor(dV => dV.DetalleVentaCreateRequest).SetValidator(new DetalleVentaCreateValidator());
        }
    }
}
