using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Productos.GetProductos;

public class GetAllProductoQuery
{
    public record GetAllProductoQueryRequest : IRequest<Result<List<ProductoResponse>>>
    {
        public GetProductosRequest? GetProductosRequest { get; set; }
    }

    internal class GetProductosQueryHandler : IRequestHandler<GetAllProductoQueryRequest, Result<List<ProductoResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProductosQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<ProductoResponse>>> Handle(GetAllProductoQueryRequest request, CancellationToken cancellationToken)
        {
            var productos = await _context.Productos
                .Include(p => p.Proveedor)
                .ProjectTo<ProductoResponse>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Result<List<ProductoResponse>>.Success(productos);
        }
    }
}
