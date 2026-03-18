using System.Text.RegularExpressions;

namespace PuntoDeVenta.Application.Utils.CustomValidations;

/// <summary>
/// Contiene métodos de validación personalizados para proveedores.
/// </summary>
public static class ProveedorCustomValidation
{
    /// <summary>
    /// Valida si la razón social proporcionada es válida según formato y tipo de sociedad.
    /// </summary>
    /// <param name="razonSocial">Razón social del proveedor.</param>
    /// <returns>true si es válida; false en caso contrario.</returns>
    public static bool IsValidRazonSocial(string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            return false;

        razonSocial = razonSocial.Trim();

        // Validar longitud razonable
        if (razonSocial.Length < 3 || razonSocial.Length > 100)
            return false;

        // Permitir letras con acentos, eñes, números, ., ,, -, &, y espacios
        // Bloquea emojis y caracteres no latinos
        string pattern = @"^[A-Za-zÁÉÍÓÚáéíóúÑñ0-9\s\.\,\-\&]+(?:\s*(S\.A\.|S\. de R\.L\.|de C\.V\.|S\.C\.|A\.C\.)\s*)*$";

        // Usamos RegexOptions para aceptar acentos de forma segura
        return Regex.IsMatch(razonSocial, pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }


    /// <summary>
    /// Valida si el nombre del proveedor es válido según formato y longitud.
    /// </summary>
    /// <param name="nombre">Nombre del proveedor.</param>
    /// <returns>true si es válido; false en caso contrario.</returns>
    public static bool IsValidNombreProveedor(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return false;

        nombre = nombre.Trim();

        if (nombre.Length < 3 || nombre.Length > 100)
            return false;

        // Rechazar si contiene múltiples espacios consecutivos
        if (Regex.IsMatch(nombre, @"\s{2,}"))
            return false;

        // Expresión regular que permite letras, números, acentos, signos comunes y espacios simples
        string pattern = @"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9.,&\-\s]+$";

        return Regex.IsMatch(nombre, pattern);
    }

    /// <summary>
    /// Valida si el RFC (Registro Federal de Contribuyentes) es válido según el formato oficial.
    /// </summary>
    /// <param name="rfc">RFC a validar.</param>
    /// <returns>true si es válido; false en caso contrario.</returns>
    public static bool IsValidRFC(string rfc)
    {
        if (string.IsNullOrWhiteSpace(rfc))
            return false;

        rfc = rfc.Trim().ToUpper();

        // Expresión regular para validar RFCs tanto de personas morales (12 caracteres)
        // como de personas físicas (13 caracteres)
        // Formato:
        //   - 3 o 4 letras (A-Z, Ñ, &) 
        //   - 6 dígitos de fecha (yyMMdd)
        //   - 3 caracteres alfanuméricos para la homoclave
        string pattern = @"^[A-ZÑ&]{3,4}\d{6}[A-Z0-9]{3}$";

        return Regex.IsMatch(rfc, pattern);
    }
}
