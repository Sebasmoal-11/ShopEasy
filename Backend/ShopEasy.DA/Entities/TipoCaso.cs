namespace ShopEasy.DA.Entities;

/// <summary>
/// Tipo/clasificacion de un caso de cliente (tabla <c>tipos_caso</c>).
/// </summary>
public class TipoCaso
{
    public int IdTipoCaso { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }

    public ICollection<CasoCliente> CasosCliente { get; set; } = new List<CasoCliente>();
}
