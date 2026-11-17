namespace ShopEasy.BW.Dashboard;

public record VentasResponse(
    int PeriodoDias,
    DateOnly Desde,
    DateOnly Hasta,
    decimal TotalVendidoPeriodo,
    int TotalPedidosPeriodo,
    decimal VentasUltimos7Dias,
    decimal VentasUltimos30Dias,
    List<VentaPorDiaDto> PorDia);

public record VentaPorDiaDto(
    DateOnly Fecha,
    int CantidadPedidos,
    decimal MontoTotal);
