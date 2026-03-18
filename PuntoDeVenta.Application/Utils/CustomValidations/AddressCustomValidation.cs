using System.Text.RegularExpressions;

namespace PuntoDeVenta.Application.Utils.CustomValidations;

public static class AddressCustomValidation
{
    public static bool IsValidStreet(string calle)
    {
        if (string.IsNullOrWhiteSpace(calle))
            return false;

        calle = calle.Trim();

        if (calle.Length < 3 || calle.Length > 100)
            return false;

        // Validar caracteres comunes en calles: letras, números, espacios, ., -, °, #, /
        string pattern = @"^[A-ZÁÉÍÓÚÑ0-9\s.,#°/\-]+$";

        return Regex.IsMatch(calle, pattern, RegexOptions.IgnoreCase);
    }

    public static bool IsValidExteriorNomber(string numeroExterior)
    {
        return true;
    }
    public static bool IsValidInteriorNumber(string numeroInterior)
    {
        return true;
    }
    public static bool IsValidCologne(string colonia)
    {
        return true;
    }
    public static bool IsValidZipCode(string codigoPostal)
    {
        return true;
    }
    public static bool IsValidCity(string ciudad)
    {
        return true;
    }
    public static bool IsValidEstate(string estado)
    {
        return true;
    }
    public static bool IsValidCountry(string pais)
    {
        return true;
    }
}
