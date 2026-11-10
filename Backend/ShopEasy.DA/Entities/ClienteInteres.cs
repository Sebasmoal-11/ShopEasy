namespace ShopEasy.DA.Entities;

/// <summary>
/// Relacion N:M entre clientes e intereses (tabla <c>cliente_intereses</c>).
/// </summary>
public class ClienteInteres
{
    public int IdCliente { get; set; }
    public int IdInteres { get; set; }
    public DateTime FechaAsignacion { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Interes Interes { get; set; } = null!;
}
