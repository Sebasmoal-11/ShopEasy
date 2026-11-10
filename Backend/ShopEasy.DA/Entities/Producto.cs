namespace ShopEasy.DA.Entities;

/// <summary>
/// Producto del catalogo (tabla <c>productos</c>).
/// </summary>
public class Producto
{
    public int IdProducto { get; set; }
    public int IdBodega { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Bodega Bodega { get; set; } = null!;

    public ICollection<DetallePedido> DetallePedido { get; set; } = new List<DetallePedido>();
}
