using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Productos.GetProductos;

public class GetProductoQuery
{
    public record GetProductoQueryRequest : IRequest<Result<ProductoResponse>>
    {
        public Guid Id { get; set; }
    }

    internal class GetProductoQueryHandler : IRequestHandler<GetProductoQueryRequest, Result<ProductoResponse>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProductoQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<ProductoResponse>> Handle(GetProductoQueryRequest request, CancellationToken cancellationToken)
        {
            var producto = await _context
                .Productos
                .Where(p => p.Id == request.Id)
                .Include(p => p.Proveedor)
                .ProjectTo<ProductoResponse>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (producto is null)
            {
                return Result<ProductoResponse>.Failure("No se encontro ese producto.");
            }

            return Result<ProductoResponse>.Success(producto);

        }
    }
}
