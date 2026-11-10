namespace ShopEasy.DA.Entities;

/// <summary>
/// Estado posible de un caso de cliente (tabla <c>estados_caso</c>).
/// </summary>
public class EstadoCaso
{
    public int IdEstadoCaso { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<CasoCliente> CasosCliente { get; set; } = new List<CasoCliente>();
}
