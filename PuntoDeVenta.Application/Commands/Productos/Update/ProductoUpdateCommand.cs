using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Productos.Update;

public class ProductoUpdateCommand
{
    public record ProductoUpdateCommandRequest(Guid Id, ProductoUpdateRequest ProductoUpdateRequest)
        : IRequest<Result<Guid>>, ICommandBase;

    internal class ProductoUpdateCommandHandler : IRequestHandler<ProductoUpdateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public ProductoUpdateCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ProductoUpdateCommandRequest request, CancellationToken cancellationToken)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == request.Id); 
            
            if (producto is null) 
                return Result<Guid>.Failure("El producto que deseas modificar no existe."); 
            
            if ((producto.Stock + request.ProductoUpdateRequest.AgregarStock) > int.MaxValue) 
            { 
                return Result<Guid>.Failure("El stock es demasiado grande, no puedes agregar más."); 
            }

            var result = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Productos
                SET 
                    nombre = {request.ProductoUpdateRequest.Nombre},
                    stock = stock + {request.ProductoUpdateRequest.AgregarStock},
                    descripcion = {request.ProductoUpdateRequest.Descripcion},
                    precio_unitario = {request.ProductoUpdateRequest.PrecioUnitario},
                    precio_venta = {request.ProductoUpdateRequest.PrecioVenta},
                    codigo_barras = {request.ProductoUpdateRequest.CodigoBarras}
                WHERE id_producto = {request.Id}
            ", cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(request.Id)
                : Result<Guid>.Failure("Ups!, por ahora no se pudo actualizar el producto, intente más tarde.");
        }
    }

    public class ProductoUpdateCommandRequestValidator : AbstractValidator<ProductoUpdateCommandRequest>
    {
        public ProductoUpdateCommandRequestValidator()
        {
            RuleFor(p => p.ProductoUpdateRequest).SetValidator(new ProductoUpdateValidator());
        }
    }
}
