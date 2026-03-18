using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using PuntoDeVenta.Application.Core;
using PuntoDeVenta.Domain.Entidades;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Proveedores.GetProveedores;

public class GetProveedoresQuery
{
    public record GetProveedoresQueryRequest : IRequest<Result<PagedList<ProveedorResponse>>>
    {
        public GetProveedoresRequest? GetProveedoresRequest { get; set; }
    }

    internal class GetProveedoresQueryHandler : IRequestHandler<GetProveedoresQueryRequest, Result<PagedList<ProveedorResponse>>>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IMapper _mapper;

        public GetProveedoresQueryHandler(PuntoDeVentaDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ProveedorResponse>>> Handle(GetProveedoresQueryRequest request, CancellationToken cancellationToken)
        {
            var filtro = request.GetProveedoresRequest;
            var predicate = ExpressionBuilder.New<Proveedor>();
            IQueryable<Proveedor> queryable = _context.Proveedores;

            if (!string.IsNullOrWhiteSpace(filtro!.RazonSocial))
            {
                predicate = predicate
                    .And(p => p.RazonSocial!.ToLower().Contains(filtro.RazonSocial.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filtro.RFC))
            {
                predicate = predicate.And(p => p.RFC!.ToLower().Contains(filtro.RFC));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Nombre))
            {
                predicate = predicate.And(p => p.Nombre!.ToLower().Contains(filtro.Nombre));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Telefono))
            {
                predicate = predicate.And(p => p.Telefono!.Contains(filtro.Telefono));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Email))
            {
                predicate = predicate.And(p => p.Email!.ToLower().Contains(filtro.Email.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Direccion))
            {
                predicate = predicate.And(p => p.Direccion!.ToLower().Contains(filtro.Direccion));
            }

            if(filtro.FechaDeRegistro.HasValue)
            {
                predicate = predicate.And(p => p.FechaDeRegistro >= filtro.FechaDeRegistro.Value);
            }

            if (!string.IsNullOrWhiteSpace(filtro.OrderBy))
            {
                Expression<Func<Proveedor, object>> orderBySelector = filtro.OrderBy.ToLower() switch
                {
                    "razonsocial" => p => p.RazonSocial!,
                    "rfc" => p => p.RFC!,
                    "nombre" => p => p.Nombre!,
                    "telefono" => p => p.Telefono!,
                    "email" => p => p.Email!,
                    "correo" => p => p.Email!,
                    "direccion" => p => p.Direccion!,
                    "fecharegistro" => p => p.FechaDeRegistro!,

                    _ => p => p.RazonSocial!
                };

                bool orderBy = filtro.OrderAscending.HasValue
                    ? filtro.OrderAscending.Value
                    : true;

                queryable = orderBy
                    ? queryable.OrderBy(orderBySelector)
                    : queryable.OrderByDescending(orderBySelector);
            }

            queryable = queryable.Where(predicate);

            var proveedoresQuery = queryable
                .ProjectTo<ProveedorResponse>(_mapper.ConfigurationProvider)
                .AsQueryable();

            var pagination = await PagedList<ProveedorResponse>.CreateAsync(
                proveedoresQuery,
                filtro.PageNumber,
                filtro.PageSize
            );

            return Result<PagedList<ProveedorResponse>>.Success(pagination);
        }
    }
}
