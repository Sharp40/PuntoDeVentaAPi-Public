using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Productos.GetProductos;

public class GetProductosQuery
{
    public record GetProductosQueryRequest : IRequest<Result<PagedList<ProductoResponse>>>
    {
        public GetProductosRequest? GetProductosRequest { get; set; }
    }

    internal class GetProductosQueryHandler : IRequestHandler<GetProductosQueryRequest, Result<PagedList<ProductoResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProductosQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ProductoResponse>>> Handle(GetProductosQueryRequest request, CancellationToken cancellationToken)
        {
            var filtro = request.GetProductosRequest;

            IQueryable<Producto> queryable = _context.Productos;
            var predicate = ExpressionBuilder.New<Producto>();

            if (!string.IsNullOrWhiteSpace(filtro!.Nombre))
                predicate = predicate.And(p => p.Nombre!.ToLower().Contains(filtro.Nombre.ToLower()));

            if (filtro.PrecioUnitario.HasValue)
                predicate = predicate.And(p => p.PrecioUnitario == filtro.PrecioUnitario.Value);

            if (filtro.PrecioVenta.HasValue)
                predicate = predicate.And(p => p.PrecioVenta == filtro.PrecioVenta.Value);

            if (filtro.Stock.HasValue)
                predicate = predicate.And(p => p.Stock == filtro.Stock.Value);

            if (filtro.StockMinimo.HasValue)
                predicate = predicate.And(p => p.Stock >= filtro.StockMinimo.Value);

            if (!string.IsNullOrWhiteSpace(filtro.Estado))
                predicate = predicate.And(p => p.Estado != null && p.Estado.ToLower() == filtro.Estado.ToLower());

            if (!string.IsNullOrWhiteSpace(filtro.Proveedor))
            {
                // Aplica filtro en el nombre del proveedor
                predicate = predicate.And(p =>
                    p.Proveedor != null &&
                    p.Proveedor.Nombre!.ToLower().Contains(filtro.Proveedor.ToLower()));
            }


            if (!string.IsNullOrWhiteSpace(filtro.OrderBy))
            {
                Expression<Func<Producto, object>> orderBySelector = filtro.OrderBy.ToLower() switch
                {
                    "nombre" => p => p.Nombre!,
                    "preciounitario" => p => p.PrecioUnitario!,
                    "precioventa" => p => p.PrecioVenta!,
                    "stock" => p => p.Stock!,
                    "proveedor" => p => p.Proveedor!.Nombre!,
                    _ => p => p.Nombre!
                };

                bool orderBy = filtro.OrderAscending.HasValue
                    ? filtro.OrderAscending.Value
                    : true;

                queryable = orderBy
                    ? queryable.OrderBy(orderBySelector)
                    : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable
                .Include(p => p.Proveedor)
                .Where(predicate);

            var productosQuery = queryable
                .ProjectTo<ProductoResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            var pagination = await PagedList<ProductoResponse>.CreateAsync(
                productosQuery,
                filtro.PageNumber,
                filtro.PageSize
            );

            return Result<PagedList<ProductoResponse>>.Success(pagination);
        }
    }
}
