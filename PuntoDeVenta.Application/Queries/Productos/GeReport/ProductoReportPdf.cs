using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PuntoDeVenta.Domain.Interfaces;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Queries.Productos.GeReport;

public class ProductoReportPdf
{
    public record ProductoReportPdfQueryRequest : IRequest<MemoryStream>;

    internal class ProductoReportPdfQueryHandler
        : IRequestHandler<ProductoReportPdfQueryRequest, MemoryStream>
    {
        private readonly PuntoDeVentaDbContext _context;
        private readonly IReportService<ProductoReportPdfResponse> _reportService;
        private readonly IMapper _mapper;

        public ProductoReportPdfQueryHandler(
            PuntoDeVentaDbContext context,
            IReportService<ProductoReportPdfResponse> reportService,
            IMapper mapper)
        {
            _context = context;
            _reportService = reportService;
            _mapper = mapper;
        }

        public async Task<MemoryStream> Handle(ProductoReportPdfQueryRequest request, CancellationToken cancellationToken)
        {
            var productosBajosEnStock = await _context
                .VistaProductosBajoStockRecientes
                .ProjectTo<ProductoReportPdfResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

            var pdfStream = await _reportService.GetPdfReportProductosBajosEnStock(productosBajosEnStock);

            return pdfStream;
        }
    }
}
