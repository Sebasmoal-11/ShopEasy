namespace ShopEasy.DA.Data;

public record PagedResult<T>(
    int Total,
    List<T> Items);
