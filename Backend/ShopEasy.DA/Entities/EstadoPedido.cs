namespace ShopEasy.DA.Entities;

/// <summary>
/// Estado posible de un pedido (tabla <c>estados_pedido</c>).
/// </summary>
public class EstadoPedido
{
    public int IdEstadoPedido { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();
}
