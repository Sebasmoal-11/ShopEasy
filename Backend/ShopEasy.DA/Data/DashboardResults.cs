namespace ShopEasy.DA.Data;

/// <summary>
/// Filas agregadas que la capa DA entrega a BC para construir el Dashboard.
/// No son entidades ni DTO de la API.
/// </summary>
public record ConteoPedidoPorDia(
    DateTime Fecha,
    int CantidadPedidos,
    decimal MontoTotal);

public record ConteoPedidoEstado(
    int IdEstadoPedido,
    string Estado,
    int Cantidad);

public record ProductoVendido(
    int IdProducto,
    string Nombre,
    int CantidadVendida,
    decimal MontoGenerado);

public record ClienteDestacado(
    int IdCliente,
    string NombreCompleto,
    string? Correo,
    int CantidadPedidos,
    decimal MontoTotal);

public record ConteoEstadoCaso(
    int IdEstadoCaso,
    string Estado,
    int Cantidad);

public record ConteoTipoCaso(
    int IdTipoCaso,
    string Tipo,
    int Cantidad);

public record ConteoCanalCaso(
    string Canal,
    int Cantidad);

public record ConteoEstadoCampana(
    string Estado,
    int Cantidad);

public record ProximaCampana(
    int IdCampana,
    string Nombre,
    string Canal,
    DateTime FechaProgramada,
    string Estado);
