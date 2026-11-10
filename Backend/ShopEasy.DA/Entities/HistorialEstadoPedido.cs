namespace ShopEasy.DA.Entities;

/// <summary>
/// Historial de cambios de estado de un pedido (tabla <c>historial_estados_pedido</c>).
/// </summary>
public class HistorialEstadoPedido
{
    public int IdHistorial { get; set; }
    public int IdPedido { get; set; }
    public int IdEstadoPedido { get; set; }
    public int? IdUsuario { get; set; }
    public DateTime FechaCambio { get; set; }
    public string? Observacion { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public EstadoPedido EstadoPedido { get; set; } = null!;
    public Usuario? Usuario { get; set; }
}
