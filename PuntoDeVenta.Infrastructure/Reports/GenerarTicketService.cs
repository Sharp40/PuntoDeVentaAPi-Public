using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PuntoDeVenta.Domain.Entidades.Otro;
using PuntoDeVenta.Domain.Interfaces;

namespace PuntoDeVenta.Infrastructure.Reports
{
    public class GenerarTicketService : IGenerarTicketService
    {
        public async Task<MemoryStream> GenerarTicket(List<TicketDetalle> detalles, TicketInfo info)
        {
            var stream = new MemoryStream();
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);

            var titleFont = new XFont("Arial", 14, XFontStyle.Bold);
            var normalFont = new XFont("Arial", 10, XFontStyle.Regular);
            var boldFont = new XFont("Arial", 10, XFontStyle.Bold);

            double y = 20;

            void DrawText(string text, XFont font, double indent = 40)
            {
                gfx.DrawString(text, font, XBrushes.Black, indent, y);
                y += font.GetHeight();
            }

            void DrawLine()
            {
                gfx.DrawString(new string('-', 60), normalFont, XBrushes.Black, 40, y);
                y += normalFont.GetHeight();
            }

            // Encabezado
            DrawText("TICKET DE VENTA", titleFont, 180);
            y += 10;

            // Información de la venta
            DrawText($"Fecha: {info.Fecha:dd/MM/yyyy hh:mm:ss tt}", normalFont);
            DrawText($"Cajero: {info.Cajero}", normalFont);
            DrawText($"Cliente: {info.Cliente}", normalFont);
            DrawText($"Teléfono: {info.Telefono}", normalFont);
            DrawText($"ID Venta: {info.IdVenta}", normalFont);
            DrawText($"Método de Pago: {info.MetodoPago}", normalFont);

            DrawLine();

            // Encabezados tabla detalle
            DrawText("Producto", boldFont);
            DrawText("Cant", boldFont);
            DrawText("P.Unit", boldFont);
            DrawText("Total", boldFont);

            DrawLine();

            // Detalle productos
            foreach (var item in detalles)
            {
                DrawText(item.Producto, normalFont);
                if (!string.IsNullOrWhiteSpace(item.Presentacion))
                    DrawText(item.Presentacion, normalFont);

                DrawText(item.Cantidad.ToString(), normalFont);
                DrawText($"{item.PrecioUnitario:C2}", normalFont);
                DrawText($"{item.Importe:C2}", normalFont);
                y += 5;
            }

            DrawLine();

            // Totales
            decimal total = detalles.Sum(d => d.Importe);
            DrawText($"TOTAL: {total:C2}", boldFont);

            if (info.MetodoPago.Equals("efectivo", StringComparison.OrdinalIgnoreCase))
            {
                DrawText($"Pagado: {info.MontoRecibido:C2}", normalFont);
                DrawText($"Cambio: {(info.MontoRecibido - total):C2}", normalFont);
            }

            DrawLine();

            DrawText("¡Gracias por su compra!", boldFont, 150);

            document.Save(stream, false);
            stream.Position = 0;

            return await Task.FromResult(stream);
        }
    }


    
}
