namespace PuntoDeVenta.Application.Core;

public abstract class PagingParams
{
    
    private const int MAX_PAGE_SIZE = 50;
    private int _pageSize = 10; // Pagina por defecto
    private int _pageNumber = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MAX_PAGE_SIZE) ? MAX_PAGE_SIZE : value;
    }

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value < 1) ? 1 : value;
    }

    public string? OrderBy { get; set; }
    public bool? OrderAscending { get; set; } = true;
}
