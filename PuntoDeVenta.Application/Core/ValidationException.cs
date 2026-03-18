namespace PuntoDeVenta.Application.Core;

public sealed class ValidationException : Exception
{
    public IEnumerable<ValidationError> Errors { get; set; }
    public ValidationException(IEnumerable<ValidationError> errors) : base("Erroes de validación")
    {
        Errors = errors;
    }
}
