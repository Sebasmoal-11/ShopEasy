namespace ShopEasy.DA.Entities;

/// <summary>
/// Canal de marketing (tabla <c>canales_marketing</c>).
/// </summary>
public class CanalMarketing
{
    public int IdCanal { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public ICollection<Campana> Campanas { get; set; } = new List<Campana>();
}
