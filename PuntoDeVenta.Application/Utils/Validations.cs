using System.Text.RegularExpressions;
using PuntoDeVenta.Domain.Constantes;
using PuntoDeVenta.Infrastructure.Persistence;

namespace PuntoDeVenta.Application.Utils;

/// <summary>
/// Contiene métodos de validación generales utilizados en la aplicación.
/// </summary>
public class Validations
{
    /// <summary>
    /// Valida si un correo electrónico es válido como el nombre de usuario, si cuenta con el caracter @, dominio etc.
    /// </summary>
    /// <param name="email">Correo electrónico a validar.</param>
    /// <returns>true si es válido; false si no lo es.</returns>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        var regex = new Regex(
            // Inicio del string, no puede comenzar con punto
            @"^(?!\.)" +
            // No se permiten dos puntos consecutivos
            @"(?!.*\.\.)" +
            // Parte local (antes de @), hasta 64 caracteres
            @"[a-zA-Z0-9._%+-]{1,64}" +
            // No puede terminar con punto
            @"(?<!\.)" +
            // Separador @
            @"@" +
            // Dominio total hasta 255 caracteres
            @"(?=.{1,255}$)" +
            // Subdominios válidos
            @"(?:(?!-)[a-zA-Z0-9-]{1,63}(?<!-)\.)+" +
            // Dominio final (ej. com, org), 2-63 letras
            @"[a-zA-Z]{2,63}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        return regex.IsMatch(email);
    }

    /// <summary>
    /// Valida si un número telefónico es válido en formato mexicano (opcional +52).
    /// </summary>
    /// <param name="phoneNumber">Número telefónico a validar.</param>
    /// <returns>true si es válido; false si no lo es.</returns>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // Eliminar caracteres comunes de formato
        string limpio = Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");

        // Formato: opcional +52 y/o 1 seguido de 10 dígitos válidos
        string pattern = @"^(\+52)?(1)?[2-9][0-9]{9}$";

        return Regex.IsMatch(limpio, pattern);
    }

    /// <summary>
    /// Verifica si un nombre de usuario ya existe en la base de datos.
    /// </summary>
    /// <param name="username">Nombre de usuario a buscar.</param>
    /// <param name="context">Contexto de base de datos.</param>
    /// <returns>true si ya existe; false si no.</returns>
    public static bool ExistsUsername(string username, PuntoDeVentaDbContext context)
    {
        return context.Usuarios.Any(u => u.NombreDeUsuario == username);
    }

    /// <summary>
    /// Valida si una contraseña es fuerte. Debe tener:
    /// al menos 8 caracteres, una mayúscula, una minúscula,
    /// un número y un carácter especial.
    /// </summary>
    /// <param name="password">Contraseña a validar.</param>
    /// <returns>true si es segura; false si no lo es.</returns>
    public static bool IsStrongPassword(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(c => "!@#$%^&*()_+-=[]{}|;':\",.<>?/`~".Contains(c));
    }

    /// <summary>
    /// Valida si el rol es uno de los permitidos (Administrador o Cajero).
    /// </summary>
    /// <param name="role">Rol a validar.</param>
    /// <returns>true si es válido; false si no lo es.</returns>
    public static bool IsValidRole(string role)
    {
        return role == Rol.Administrador || role == Rol.Cajero;
    }

    /// <summary>
    /// Valida si un nombre es válido en cuanto a formato y caracteres permitidos.
    /// </summary>
    /// <param name="name">Nombre a validar.</param>
    /// <returns>true si es válido; false si no lo es.</returns>
    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        name = name.Trim();

        if (name.Length < 3 || name.Length > 50)
            return false;

        var regex = new Regex(
            // No puede iniciar con espacio, guion o apóstrofe
            @"^(?![-' ])" +
            // No permite dobles guiones o apóstrofes
            @"(?!.*[-']{2})" +
            // No permite espacios dobles
            @"(?!.*\s{2})" +
            // Letras, acentos, espacios, guiones y apóstrofes
            @"[A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s'-]+" +
            // No puede terminar en espacio, guion o apóstrofe
            @"(?<![-' ])$",
            RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled
        );

        return regex.IsMatch(name);
    }

    /// <summary>
    /// Valida si un apellido es válido. Usa la misma validación que los nombres.
    /// </summary>
    /// <param name="lastName">Apellido a validar.</param>
    /// <returns>true si es válido; false si no lo es.</returns>
    public static bool IsValidLastName(string lastName)
    {
        return IsValidName(lastName);
    }

    
}
