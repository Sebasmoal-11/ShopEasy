namespace ShopEasy.DA.Entities;

/// <summary>
/// Preferencia de publicidad de un cliente (tabla <c>preferencias_publicidad</c>).
/// </summary>
public class PreferenciaPublicidad
{
    public int IdPreferenciaPublicidad { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
}
