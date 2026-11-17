namespace ShopEasy.BW.Pedidos;

public class CrearPedidoRequest
{
    public int IdCliente { get; set; }
    public string DireccionEnvio { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
    public List<DetallePedidoRequest> Detalles { get; set; } = new();
}

public class DetallePedidoRequest
{
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
}

public class ActualizarEstadoPedidoRequest
{
    public int IdEstadoPedido { get; set; }
    public string? Observacion { get; set; }
}

public record EstadoPedidoDto(
    int IdEstadoPedido,
    string Nombre,
    string? Descripcion);

public record PedidoItemDto(
    int IdProducto,
    string Producto,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal);

public record HistorialEstadoPedidoDto(
    int IdHistorial,
    int IdEstadoPedido,
    string Estado,
    DateTime FechaCambio,
    int? IdUsuario,
    string? Usuario,
    string? Observacion);

public record PedidoResumenDto(
    int IdPedido,
    int IdCliente,
    string Cliente,
    string Cedula,
    int IdEstadoPedido,
    string Estado,
    DateTime FechaPedido,
    decimal Total,
    string DireccionEnvio,
    int CantidadProductos);

public record PedidoDetalleResponse(
    int IdPedido,
    int IdCliente,
    string Cliente,
    string Cedula,
    string? CorreoCliente,
    int IdEstadoPedido,
    string Estado,
    DateTime FechaPedido,
    decimal Total,
    string DireccionEnvio,
    string? Observaciones,
    List<PedidoItemDto> Detalles,
    List<HistorialEstadoPedidoDto> Historial);
