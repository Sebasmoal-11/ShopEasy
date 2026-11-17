namespace ShopEasy.BW.Dashboard;

public record PedidosEstadosResponse(
    int TotalPedidos,
    List<PedidoEstadoDto> Estados);

public record PedidoEstadoDto(
    int IdEstadoPedido,
    string Estado,
    int Cantidad);
