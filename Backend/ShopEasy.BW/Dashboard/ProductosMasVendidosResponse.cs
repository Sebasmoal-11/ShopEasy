namespace ShopEasy.BW.Dashboard;

public record ProductosMasVendidosResponse(
    List<ProductoMasVendidoDto> Productos);

public record ProductoMasVendidoDto(
    int IdProducto,
    string Nombre,
    int CantidadVendida,
    decimal MontoGenerado);
