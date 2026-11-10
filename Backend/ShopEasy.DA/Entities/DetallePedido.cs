namespace ShopEasy.DA.Entities;

/// <summary>
/// Linea de detalle de un pedido (tabla <c>detalle_pedido</c>).
/// </summary>
public class DetallePedido
{
    public int IdDetalle { get; set; }
    public int IdPedido { get; set; }
    public int IdProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
