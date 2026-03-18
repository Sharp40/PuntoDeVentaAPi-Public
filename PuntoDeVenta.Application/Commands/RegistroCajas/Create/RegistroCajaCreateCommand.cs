using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.RegistroCajas.Create;

public class RegistroCajaCreateCommand
{
    public record RegistroCajaCreateCommandRequest(RegistroCajaCreateRequest RegistroCajaRequest) 
        : IRequest<Result<Guid>>;

    internal class RegistroCajaCommandHandler 
        : IRequestHandler<RegistroCajaCreateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public RegistroCajaCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(RegistroCajaCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var registroCajaId = Guid.NewGuid();

            var registroCaja = new RegistroCaja
            {
                Id = registroCajaId,
                FechaHora = request.RegistroCajaRequest.FechaHora,
                MetodoPago = request.RegistroCajaRequest.MetodoPago,
                MontoPagado = request.RegistroCajaRequest.MontoPagado,
                CambioEntregado = request.RegistroCajaRequest.CambioEntregado,
                TotalVenta = request.RegistroCajaRequest.TotalVenta,
                Usuario = request.RegistroCajaRequest.Usuario,
                IdVenta = request.RegistroCajaRequest.IdVenta
            };

            _context.Add(registroCaja);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result 
                ? Result<Guid>.Success(registroCajaId)
                : Result<Guid>.Failure("Error al registrar el movimiento en caja.");
        }
    }

    public class RegistroCajaCreateCommandRequestValidator : AbstractValidator<RegistroCajaCreateCommandRequest>
    {
        public RegistroCajaCreateCommandRequestValidator()
        {
            RuleFor(r => r.RegistroCajaRequest).SetValidator(new RegistroCajaCreateValidator());
        }
    }
}
