using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Proveedores.Create;

public class ProveedorCreateCommand
{
    public record ProveedorCreateCommmandRequest(ProveedorCreateRequest ProveedorCreateRequest)
        : IRequest<Result<Guid>>, ICommandBase;

    internal class ProveedorCreateCommandHandler : IRequestHandler<ProveedorCreateCommmandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public ProveedorCreateCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ProveedorCreateCommmandRequest request, CancellationToken cancellationToken)
        {
            var proveedorId = Guid.NewGuid();

            var proveedor = new Proveedor
            {
                Id = proveedorId,
                RazonSocial = request.ProveedorCreateRequest.RazonSocial,
                Nombre = request.ProveedorCreateRequest.Nombre,
                RFC = request.ProveedorCreateRequest.RFC,
                Telefono = request.ProveedorCreateRequest.Telefono,
                Email = request.ProveedorCreateRequest.Email,
                Direccion = request.ProveedorCreateRequest.Direccion,
                FechaDeRegistro = DateTime.UtcNow
            };

            // Agregar context para guardar el la bd
            _context.Add(proveedor);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(proveedor.Id)
                : Result<Guid>.Failure("Ups!, Por ahora no se pudo agregar el nuevo proveedor, intentelo más tarde.");

            //return Task.FromResult(Result<Proveedor>.Success(proveedor));//Test
        }
    }

    public class ProveedorCreateCommandRequestValidator : AbstractValidator<ProveedorCreateCommmandRequest>
    {
        public ProveedorCreateCommandRequestValidator()
        {
            RuleFor(p => p.ProveedorCreateRequest).SetValidator(new ProveedorCreateValidator());
        }
    }
}
