namespace ShopEasy.BW.Common;

public record PagedResponse<T>(
    int Total,
    int Pagina,
    int Tamano,
    List<T> Items);
