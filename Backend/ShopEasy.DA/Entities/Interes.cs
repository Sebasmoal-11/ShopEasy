namespace ShopEasy.DA.Entities;

/// <summary>
/// Interes comercial (tabla <c>intereses</c>).
/// </summary>
public class Interes
{
    public int IdInteres { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; }

    public ICollection<ClienteInteres> ClienteIntereses { get; set; } = new List<ClienteInteres>();
    public ICollection<CampanaInteres> CampanaIntereses { get; set; } = new List<CampanaInteres>();
}
