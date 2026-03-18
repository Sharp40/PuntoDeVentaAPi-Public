using System.Data;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using Microsoft.Data.SqlClient;
using Dapper; // Asegúrate de instalar Dapper


namespace PuntoDeVenta.Infrastructure.Reports;

public class HistorialDeVentas
{
    private readonly string _connectionString;

    public HistorialDeVentas(string connectionString)
    {
        _connectionString = connectionString;
    }

    // ========================
    // 📄 Reporte Diario
    // ========================
    public byte[] GenerarReporteDiario(DateTime fecha)
    {
        var dt = new DataTable();
        decimal total = 0;

        using var con = new SqlConnection(_connectionString);
        con.Open();

        // Detalle
        using (var cmd = new SqlCommand("SP_VentasDetallePorDia", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        // Total
        using (var cmd = new SqlCommand("SP_SumaTotalVendidoPorDia", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            total = Convert.ToDecimal(cmd.ExecuteScalar());
        }

        return GenerarPdf("Reporte Diario", dt, total);
    }

    // ========================
    // 📄 Reporte Semanal
    // ========================
    public byte[] GenerarReporteSemanal(DateTime fecha)
    {
        var dt = new DataTable();
        decimal total = 0;

        using var con = new SqlConnection(_connectionString);
        con.Open();

        string query = @"
            SELECT CAST(fecha AS DATE) AS Dia, SUM(total) AS TotalPorDia
            FROM Ventas
            WHERE DATEPART(iso_week, fecha) = DATEPART(iso_week, @Fecha)
              AND DATEPART(year, fecha) = DATEPART(year, @Fecha)
            GROUP BY CAST(fecha AS DATE)
            ORDER BY Dia";

        using (var cmd = new SqlCommand(query, con))
        {
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        using (var cmd = new SqlCommand("SP_SumaTotalVendidoPorSemana", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Fecha", fecha);
            total = Convert.ToDecimal(cmd.ExecuteScalar());
        }

        return GenerarPdf("Reporte Semanal", dt, total);
    }

    // ========================
    // 📄 Reporte Mensual
    // ========================
    public byte[] GenerarReporteMensual(int anio, int mes)
    {
        var dt = new DataTable();
        decimal total = 0;

        using var con = new SqlConnection(_connectionString);
        con.Open();

        using (var cmd = new SqlCommand("SP_VentasPorSemanaLunesADomingoEnMes", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Anio", anio);
            cmd.Parameters.AddWithValue("@Mes", mes);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        using (var cmd = new SqlCommand("SP_SumaTotalVendidoPorMes", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Anio", anio);
            cmd.Parameters.AddWithValue("@Mes", mes);
            total = Convert.ToDecimal(cmd.ExecuteScalar());
        }

        return GenerarPdf("Reporte Mensual", dt, total);
    }

    // ========================
    // 📄 Reporte Anual
    // ========================
    public byte[] GenerarReporteAnual(int anio)
    {
        var dt = new DataTable();

        using var con = new SqlConnection(_connectionString);
        con.Open();

        using (var cmd = new SqlCommand("SP_SumaTotalVendidoPorMesesDelAño", con))
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Anio", anio);
            using var da = new SqlDataAdapter(cmd);
            da.Fill(dt);
        }

        return GenerarPdf("Reporte Anual", dt);
    }

    // ========================
    // 📄 Reporte Stock
    // ========================
    public byte[] GenerarReporteStock()
    {
        var dt = new DataTable();

        using var con = new SqlConnection(_connectionString);
        con.Open();

        string query = @"
            SELECT 
                p.nombre As Producto,
                p.precio_unitario,
                p.stock,
                p.codigo_barras,
                p.descripcion,
                pr.empresa_RazonSocial AS Empresa,
                p.precio_venta,
                p.Estado,
                CASE
                    WHEN p.stock = 0 THEN 'Agotado'
                    WHEN p.stock <= 2 THEN 'Bajo stock'
                    ELSE 'Disponible'
                END AS EstadoStock
            FROM Productos p
            LEFT JOIN Proveedores pr ON p.id_proveedor = pr.id_proveedor
            WHERE p.Estado = 'Activo'
            ORDER BY p.nombre";

        using (var cmd = new SqlCommand(query, con))
        using (var da = new SqlDataAdapter(cmd))
        {
            da.Fill(dt);
        }

        return GenerarPdfReporteStock(dt);
    }

    // ========================
    // 📄 Productos Más Vendidos
    // ========================
    public byte[] GenerarReporteProductosMasVendidos()
    {
        var dt = new DataTable();
        decimal total = 0;

        using var con = new SqlConnection(_connectionString);
        con.Open();

        // Consulta propia, usando LEFT JOIN si quieres incluir productos sin ventas
        string query = @"
        SELECT TOP 10
            p.nombre AS Producto,
            ISNULL(SUM(dv.cantidad), 0) AS TotalVendido
        FROM Productos p
        LEFT JOIN Detalle_Ventas dv ON p.id_producto = dv.id_producto
        GROUP BY p.nombre
        ORDER BY TotalVendido DESC";

        using (var cmd = new SqlCommand(query, con))
        using (var da = new SqlDataAdapter(cmd))
        {
            da.Fill(dt);
        }

        // Total de ventas de los productos listados
        total = dt.AsEnumerable().Sum(row => Convert.ToDecimal(row["TotalVendido"]));

        return GenerarPdf("Productos Más Vendidos", dt, total);
    }

    // ========================
    // 📄 Productos Menos Vendidos
    // ========================
    public byte[] GenerarReporteProductosMenosVendidos()
    {
        var dt = new DataTable();
        decimal total = 0;

        using var con = new SqlConnection(_connectionString);
        con.Open();

        string query = @"
        SELECT TOP 10
            p.nombre AS Producto,
            ISNULL(SUM(dv.cantidad), 0) AS TotalVendido
        FROM Productos p
        LEFT JOIN Detalle_Ventas dv ON p.id_producto = dv.id_producto
        GROUP BY p.nombre
        ORDER BY TotalVendido ASC";

        using (var cmd = new SqlCommand(query, con))
        using (var da = new SqlDataAdapter(cmd))
        {
            da.Fill(dt);
        }

        // Total de ventas de los productos listados
        total = dt.AsEnumerable().Sum(row => Convert.ToDecimal(row["TotalVendido"]));

        return GenerarPdf("Productos Menos Vendidos", dt, total);
    }


    // ========================
    // 🛠️ Generador de PDF mejorado
    // ========================
    private byte[] GenerarPdf(string titulo, DataTable dt, decimal? total = null)
    {
        using var doc = new PdfDocument();
        var page = doc.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var fontTitle = new XFont("Helvetica", 16, XFontStyle.Bold);
        var fontHeader = new XFont("Helvetica", 10, XFontStyle.Bold);
        var fontRow = new XFont("Helvetica", 10, XFontStyle.Regular);

        double margen = 40;
        double y = 40;

        gfx.DrawString(titulo, fontTitle, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
        y += 40;

        // Ancho dinámico por columna
        double anchoColumna = (page.Width - 2 * margen) / dt.Columns.Count;

        // Encabezados
        double x = margen;
        foreach (DataColumn col in dt.Columns)
        {
            gfx.DrawString(col.ColumnName, fontHeader, XBrushes.Black, new XRect(x, y, anchoColumna, 20), XStringFormats.TopLeft);
            x += anchoColumna;
        }
        y += 20;

        // Filas con paginación
        foreach (DataRow row in dt.Rows)
        {
            if (y > page.Height - 60) // nuevo page si se pasa del límite
            {
                page = doc.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
            }

            x = margen;
            foreach (var item in row.ItemArray)
            {
                gfx.DrawString(item!.ToString(), fontRow, XBrushes.Black, new XRect(x, y, anchoColumna, 20), XStringFormats.TopLeft);
                x += anchoColumna;
            }
            y += 20;
        }

        // Total
        if (total.HasValue)
        {
            y += 20;
            gfx.DrawString($"TOTAL: {total.Value:C}", fontTitle, XBrushes.Black, new XRect(margen, y, page.Width - 2 * margen, 20), XStringFormats.TopLeft);
        }

        using var ms = new MemoryStream();
        doc.Save(ms, false);
        return ms.ToArray();
    }

    private byte[] GenerarPdfReporteStock(DataTable dt)
    {
        using var doc = new PdfDocument();
        var page = doc.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var fontTitle = new XFont("Helvetica", 16, XFontStyle.Bold);
        var fontHeader = new XFont("Helvetica", 10, XFontStyle.Bold);
        var fontRow = new XFont("Helvetica", 9, XFontStyle.Regular);

        double margen = 40;
        double y = 40;

        // Título centrado
        gfx.DrawString("Reporte Stock", fontTitle, XBrushes.Black, new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
        y += 40;

        // Definir anchos personalizados para cada columna
        double[] anchos = { 80, 60, 40, 80, 120, 90, 60, 50, 70 };
        double x = margen;

        // Encabezados con fondo y borde
        for (int i = 0; i < dt.Columns.Count; i++)
        {
            var rect = new XRect(x, y, anchos[i], 22);
            gfx.DrawRectangle(XBrushes.LightGray, rect);
            gfx.DrawRectangle(XPens.Black, rect);
            gfx.DrawString(dt.Columns[i].ColumnName, fontHeader, XBrushes.Black, rect, XStringFormats.Center);
            x += anchos[i];
        }
        y += 22;

        // Filas
        foreach (DataRow row in dt.Rows)
        {
            if (y > page.Height - 60)
            {
                page = doc.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                y = 40;
            }

            x = margen;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var item = row[i]?.ToString() ?? "";
                var rect = new XRect(x, y, anchos[i], 20);

                // Fondo alterno
                if (dt.Rows.IndexOf(row) % 2 == 1)
                    gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(245, 245, 245)), rect);

                gfx.DrawRectangle(XPens.Black, rect);

                // Truncar texto si es muy largo
                string textoMostrar = item;
                var size = gfx.MeasureString(item, fontRow);
                if (size.Width > anchos[i] - 6)
                {
                    int maxChars = Math.Max(0, (int)(item.Length * (anchos[i] - 12) / size.Width));
                    textoMostrar = item.Substring(0, maxChars) + "...";
                }

                // Alineación
                XStringFormat formato = XStringFormats.CenterLeft;
                if (dt.Columns[i].DataType == typeof(int) || dt.Columns[i].DataType == typeof(decimal))
                    formato = XStringFormats.CenterRight;
                if (dt.Columns[i].ColumnName.ToLower().Contains("stock") || dt.Columns[i].ColumnName.ToLower().Contains("precio"))
                    formato = XStringFormats.CenterRight;

                gfx.DrawString(textoMostrar, fontRow, XBrushes.Black, rect, formato);
                x += anchos[i];
            }
            y += 20;
        }

        using var ms = new MemoryStream();
        doc.Save(ms, false);
        return ms.ToArray();
    }

    public IEnumerable<HistorialDto> ObtenerHistorialDatos()
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        string query = "SELECT Id_Venta, Fecha, Total, MetodoPago FROM vw_HistorialVentas"; // tu vista
        return con.Query<HistorialDto>(query);
    }

    public IEnumerable<VentaProductoDto> ObtenerMasVendidoDatos()
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        string query = @"
        SELECT TOP 5
            p.nombre AS Producto,
            ISNULL(SUM(dv.cantidad), 0) AS TotalCantidad
        FROM Productos p
        LEFT JOIN Detalle_Ventas dv ON p.id_producto = dv.id_producto
        GROUP BY p.nombre
        ORDER BY TotalCantidad DESC";
        return con.Query<VentaProductoDto>(query);
    }

    public IEnumerable<VentaProductoDto> ObtenerMenosVendidoDatos()
    {
        using var con = new SqlConnection(_connectionString);
        con.Open();
        string query = @"
        SELECT TOP 5
            p.nombre AS Producto,
            ISNULL(SUM(dv.cantidad), 0) AS TotalCantidad
        FROM Productos p
        LEFT JOIN Detalle_Ventas dv ON p.id_producto = dv.id_producto
        GROUP BY p.nombre
        ORDER BY TotalCantidad ASC";
        return con.Query<VentaProductoDto>(query);
    }

}
public class HistorialDto
{
    public Guid Id_Venta { get; set; }

    public DateTime Fecha { get; set; }
    public string? MetodoPago { get; set; }
    public decimal Total { get; set; }
}

public class VentaProductoDto
{
    public string? Producto { get; set; }
    public int TotalCantidad { get; set; }
}
