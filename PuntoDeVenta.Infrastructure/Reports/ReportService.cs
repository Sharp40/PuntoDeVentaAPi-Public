using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PuntoDeVenta.Domain.Interfaces;

namespace PuntoDeVenta.Infrastructure.Reports;

public class ReportService<T> : IReportService<T> where T : class
{
    public Task<MemoryStream> GetCsvReport(List<T> records)
    {
        throw new NotImplementedException();
    }

    public Task<MemoryStream> GetPdfReport(List<T> records)
    {
        throw new NotImplementedException();
    }

    public async Task<MemoryStream> GetPdfReportProductosBajosEnStock(List<T> records)
    {
        var stream = new MemoryStream();
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var titleFont = new XFont("Arial", 16, XFontStyle.Bold);
        var headerFont = new XFont("Arial", 12, XFontStyle.Bold);
        var font = new XFont("Arial", 10, XFontStyle.Regular);

        double margin = 40;
        double yPoint = 40;
        double rowHeight = 40;

        // Columnas con ancho ajustado (que sumen dentro del ancho del contenido)
        var columns = new[]
        {
            new { Title = "Producto", Width = 120.0, Selector = new Func<T, string?>(item => (item as dynamic)?.Nombre) },
            new { Title = "Código de Barras", Width = 110.0, Selector = new Func<T, string?>(item => (item as dynamic)?.CodigoDeBarras) },
            new { Title = "Descripción", Width = 130.0, Selector = new Func<T, string?>(item => (item as dynamic)?.Descripcion) },
            new { Title = "Existencias", Width = 80.0, Selector = new Func<T, string?>(item => (item as dynamic)?.Stock.ToString()) },
            new { Title = "Fecha de Alerta", Width = 120.0, Selector = new Func<T, string?>(item =>
                (item as dynamic)?.FechaAlerta?.ToString("dd/MM/yyyy hh:mm:ss tt") ?? "")
            }
        };

        double totalTableWidth = columns.Sum(c => c.Width);
        double xStart = (page.Width - totalTableWidth) / 2; // Centrar tabla

        // Título
        gfx.DrawString("Productos Bajos en Stock", titleFont, XBrushes.Black,
            new XRect(0, yPoint, page.Width, 30), XStringFormats.TopCenter);
        yPoint += 30;

        // Fecha de elaboración
        gfx.DrawString($"Fecha de elaboración: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", font, XBrushes.Black,
            new XRect(xStart, yPoint, page.Width - xStart * 2, 20), XStringFormats.TopLeft);
        yPoint += 30;

        // Dibujar encabezado
        double x = xStart;
        foreach (var col in columns)
        {
            gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, yPoint, col.Width, rowHeight);
            gfx.DrawString(col.Title, headerFont, XBrushes.Black,
                new XRect(x + 2, yPoint + 3, col.Width, rowHeight), XStringFormats.TopLeft);
            x += col.Width;
        }

        yPoint += rowHeight;

        // Dibujar filas
        foreach (var item in records)
        {
            x = xStart;

            // Calcular altura máxima requerida por esta fila
            double maxHeight = rowHeight;

            foreach (var col in columns)
            {
                string? text = col.Selector(item) ?? "";
                var size = gfx.MeasureString(text, font);

                // Estimar líneas (para texto largo como descripción)
                int lines = (int)Math.Ceiling(size.Width / col.Width);
                double heightNeeded = lines * font.GetHeight() + 6;

                if (heightNeeded > maxHeight)
                    maxHeight = heightNeeded;
            }

            // Dibujar cada celda con altura ajustada
            foreach (var col in columns)
            {
                string? text = col.Selector(item) ?? "";

                var cellRect = new XRect(x, yPoint, col.Width, maxHeight);
                gfx.DrawRectangle(XPens.Black, cellRect);

                if (col.Title == "Existencias")
                {
                    // Para cantidad, centrar el texto sin wrap
                    gfx.DrawString(text, font, XBrushes.Black, cellRect, XStringFormats.Center);
                }
                else
                {
                    DrawStringWrap(gfx, text, font, XBrushes.Black, cellRect);
                }

                x += col.Width;
            }

            yPoint += maxHeight;

            // Si se pasa del final de la página, crear una nueva
            if (yPoint > page.Height - margin)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                yPoint = margin;

                // Redibujar encabezado
                x = xStart;
                foreach (var col in columns)
                {
                    gfx.DrawRectangle(XPens.Black, XBrushes.LightGray, x, yPoint, col.Width, rowHeight);
                    gfx.DrawString(col.Title, headerFont, XBrushes.Black,
                        new XRect(x + 2, yPoint + 3, col.Width, rowHeight), XStringFormats.TopLeft);
                    x += col.Width;
                }

                yPoint += rowHeight;
            }
        }

        document.Save(stream, false);
        stream.Position = 0;

        return await Task.FromResult(stream);
    }

    private void DrawStringWrap(XGraphics gfx, string text, XFont font, XBrush brush, XRect rect)
    {
        var lineHeight = font.GetHeight();
        var words = text.Split(' ');
        var line = "";
        double y = rect.Top;

        foreach (var word in words)
        {
            var testLine = (line.Length == 0) ? word : line + " " + word;
            var size = gfx.MeasureString(testLine, font);

            if (size.Width > rect.Width)
            {
                gfx.DrawString(line, font, brush, new XRect(rect.X + 2, y, rect.Width, lineHeight), XStringFormats.TopLeft);
                line = word;
                y += lineHeight;
            }
            else
            {
                line = testLine;
            }
        }

        // Draw last line
        if (!string.IsNullOrEmpty(line))
        {
            gfx.DrawString(line, font, brush, new XRect(rect.X + 2, y, rect.Width, lineHeight), XStringFormats.TopLeft);
        }
    }
}
