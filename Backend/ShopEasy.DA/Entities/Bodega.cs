namespace ShopEasy.DA.Entities;

/// <summary>
/// Bodega de inventario (tabla <c>bodegas</c>).
/// </summary>
public class Bodega
{
    public int IdBodega { get; set; }
    public string Nombre { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
