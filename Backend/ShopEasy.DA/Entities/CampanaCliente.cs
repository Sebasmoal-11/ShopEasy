namespace ShopEasy.DA.Entities;

/// <summary>
/// Cliente destinatario de una campana (tabla <c>campana_clientes</c>).
/// </summary>
public class CampanaCliente
{
    public int IdCampanaCliente { get; set; }
    public int IdCampana { get; set; }
    public int IdCliente { get; set; }
    public DateTime? FechaEnvio { get; set; }
    public string EstadoEnvio { get; set; } = null!;

    public Campana Campana { get; set; } = null!;
    public Cliente Cliente { get; set; } = null!;
}
