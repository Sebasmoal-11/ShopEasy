namespace ShopEasy.DA.Entities;

/// <summary>
/// Pedido realizado por un cliente (tabla <c>pedidos</c>).
/// </summary>
public class Pedido
{
    public int IdPedido { get; set; }
    public int IdCliente { get; set; }
    public int IdEstadoPedido { get; set; }
    public DateTime FechaPedido { get; set; }
    public decimal Total { get; set; }
    public string DireccionEnvio { get; set; } = null!;
    public string? Observaciones { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public EstadoPedido EstadoPedido { get; set; } = null!;

    public ICollection<DetallePedido> DetallePedido { get; set; } = new List<DetallePedido>();
    public ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();
}
