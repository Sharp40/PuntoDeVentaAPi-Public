using FluentValidation;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Domain.Enums;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Commands.Productos.Create;

public class ProductoCreateCommand
{
    public record ProductoCreateCommandRequest(ProductoCreateRequest ProductoCreateRequest)
        : IRequest<Result<Guid>>, ICommandBase;

    internal class ProductoCreateCommandHandler : 
        IRequestHandler<ProductoCreateCommandRequest, Result<Guid>>
    {
        private readonly PuntoDeVentaDbContext _context;

        public ProductoCreateCommandHandler(PuntoDeVentaDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Guid>> Handle(ProductoCreateCommandRequest request, CancellationToken cancellationToken)
        {
            var productoId = Guid.NewGuid();

            var producto = new Producto
            {
                Id = productoId,
                Nombre = request.ProductoCreateRequest.Nombre,
                PrecioUnitario = request.ProductoCreateRequest.PrecioUnitario,
                PrecioVenta = request.ProductoCreateRequest.PrecioVenta,
                Stock = request.ProductoCreateRequest.Stock,
                CodigoBarras = request.ProductoCreateRequest.CodigoBarras,
                Descripcion = request.ProductoCreateRequest.Descripcion,
                Estado = EstadoProducto.Activo.ToString(),
                StockMinimo = 0,
                ProveedorId = request.ProductoCreateRequest.ProveedorId
            };

            _context.Add(producto);
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return result
                ? Result<Guid>.Success(productoId)
                : Result<Guid>.Failure("Ups!, Por ahora no se pudo agregar el nuevo producto, intentelo más tarde.");
            //return Task.FromResult(Result<Producto>.Success(producto));
        }
    }

    public class ProductoCreateCommandRequestValidator : AbstractValidator<ProductoCreateCommandRequest>
    {
        public ProductoCreateCommandRequestValidator(PuntoDeVentaDbContext context)
        {
            RuleFor(p => p.ProductoCreateRequest).SetValidator(new ProductoCreateValidator(context));
        }
    }
}
