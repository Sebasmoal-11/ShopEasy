namespace ShopEasy.DA.Entities;

/// <summary>
/// Relacion N:M entre campanas e intereses (tabla <c>campana_intereses</c>).
/// </summary>
public class CampanaInteres
{
    public int IdCampana { get; set; }
    public int IdInteres { get; set; }

    public Campana Campana { get; set; } = null!;
    public Interes Interes { get; set; } = null!;
}
