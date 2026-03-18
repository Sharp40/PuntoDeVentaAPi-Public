using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Proveedores.Update;

public class ProveedorUpdateCommand
{
    public record ProveedorUpdateCommandRequest(Guid Id, ProveedorUpdateRequest ProveedorUpdateRequest)
        : IRequest<Result<Guid>>, ICommandBase;

    internal class ProveedorUpdateCommandHandler
        : IRequestHandler<ProveedorUpdateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public ProveedorUpdateCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ProveedorUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            var proveedorId = request.Id;

            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.Id == proveedorId, cancellationToken);

            if (proveedor is null) return Result<Guid>.Failure("El proveedor no existe.");

            proveedor.RazonSocial = request.ProveedorUpdateRequest.RazonSocial;
            proveedor.RFC = request.ProveedorUpdateRequest.RFC;
            proveedor.Nombre = request.ProveedorUpdateRequest.Nombre;
            proveedor.Telefono = request.ProveedorUpdateRequest.Telefono;
            proveedor.Email = request.ProveedorUpdateRequest.Email;
            proveedor.Direccion = request.ProveedorUpdateRequest.Direccion;

            _context.Entry(proveedor).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync() > 0;

            return result
                ? Result<Guid>.Success(proveedorId)
                : Result<Guid>.Failure("Ups!, Por ahora no se pudo actualizar el proveedor, intentelo más tarde.");
        }
    }

    public class ProveedorUpdateCommandRequestValidator : AbstractValidator<ProveedorUpdateCommandRequest>
    {
        public ProveedorUpdateCommandRequestValidator()
        {
            RuleFor(p => p.Id)
                .NotNull().WithMessage("El Id no debe ser nulo.")
                .NotEmpty().WithMessage("El Id no debe estar vacio.");

            RuleFor(p => p.ProveedorUpdateRequest).SetValidator(new ProveedorUpdateValidator());
        }
    }
}
