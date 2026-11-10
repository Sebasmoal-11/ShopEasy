namespace ShopEasy.DA.Entities;

/// <summary>
/// Rol del sistema (tabla <c>roles</c>).
/// </summary>
public class Rol
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
