namespace ShopEasy.DA.Entities;

/// <summary>
/// Cliente del CRM (tabla <c>clientes</c>).
/// </summary>
public class Cliente
{
    public int IdCliente { get; set; }
    public string Cedula { get; set; } = null!;
    public int IdPreferenciaPublicidad { get; set; }
    public DateTime FechaRegistro { get; set; }
    public bool Activo { get; set; }

    public Persona Persona { get; set; } = null!;
    public PreferenciaPublicidad PreferenciaPublicidad { get; set; } = null!;

    public ICollection<ClienteInteres> ClienteIntereses { get; set; } = new List<ClienteInteres>();
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<CasoCliente> CasosCliente { get; set; } = new List<CasoCliente>();
    public ICollection<CampanaCliente> CampanaClientes { get; set; } = new List<CampanaCliente>();
}
