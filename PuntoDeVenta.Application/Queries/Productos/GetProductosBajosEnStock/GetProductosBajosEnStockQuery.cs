using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Productos.GetProductosBajosEnStock;

public class GetProductosBajosEnStockQuery
{
    public record GetProductosBajosEnStockQueryRequest 
        : IRequest<Result<PagedList<ProductosBajosEnStockResponse>>>
    {
        public GetProductosBajosEnStockRequest? GetProductosBajosEnStockRequest { get; set; }
    }

    internal class GetProductosBajosEnStockQueryHandler
        : IRequestHandler<GetProductosBajosEnStockQueryRequest, Result<PagedList<ProductosBajosEnStockResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProductosBajosEnStockQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ProductosBajosEnStockResponse>>> Handle(GetProductosBajosEnStockQueryRequest request, CancellationToken cancellationToken)
        {
            var filtro = request.GetProductosBajosEnStockRequest;

            var predicate = ExpressionBuilder.New<ProductosBajosEnStock>();

            IQueryable<ProductosBajosEnStock> queryable = 
                _context.VistaProductosBajoStockRecientes.AsNoTracking();



            if (!string.IsNullOrWhiteSpace(filtro!.Nombre))
                predicate = predicate.And(p => p.Nombre!.ToLower().Contains(filtro!.Nombre));

            if (!string.IsNullOrWhiteSpace(filtro.CodigoDeBarras))
                predicate = predicate.And(p => p.CodigoDeBarras!.ToLower().Contains(filtro.CodigoDeBarras));

            if (filtro.Stock.HasValue)
                predicate = predicate.And(p => p.Stock >= filtro.Stock.Value);

            if (filtro.FechaAlerta.HasValue)
                predicate = predicate.And(p => p.FechaAlerta >= filtro.FechaAlerta);

            if (!string.IsNullOrWhiteSpace(filtro.Mensaje))
                predicate = predicate.And(p => p.Mensaje!.ToLower().Contains(filtro.Mensaje));

            if (!string.IsNullOrWhiteSpace(filtro.OrderBy))
            {
                Expression<Func<ProductosBajosEnStock, object>> orderBySelector = 
                    filtro.OrderBy.ToLower() switch
                {
                    "nombre" => p => p.Nombre!,
                    "codigodebarras" => p => p.CodigoDeBarras!,
                    "stock" => p => p.Stock!,
                    "fechaalerta" => p => p.FechaAlerta!,
                    "mensaje" => p => p.Mensaje!,
                    "estado" => p => p.Estado!,
                    _ => p => p.Stock!
                };

                bool orderBy = filtro.OrderAscending.HasValue
                    ? filtro.OrderAscending.Value
                    : true;

                queryable = orderBy
                    ? queryable.OrderBy(orderBySelector)
                    : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var productosBajosEnStockQuery = queryable
                .ProjectTo<ProductosBajosEnStockResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            var pagination = await PagedList<ProductosBajosEnStockResponse>.CreateAsync(
                productosBajosEnStockQuery,
                filtro.PageNumber,
                filtro.PageSize
            );

            return Result<PagedList<ProductosBajosEnStockResponse>>.Success(pagination);
        }
    }
}
