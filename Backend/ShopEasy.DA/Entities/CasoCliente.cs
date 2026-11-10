namespace ShopEasy.DA.Entities;

/// <summary>
/// Caso o incidencia de atencion al cliente (tabla <c>casos_cliente</c>).
/// </summary>
public class CasoCliente
{
    public int IdCaso { get; set; }
    public int IdCliente { get; set; }
    public int IdTipoCaso { get; set; }
    public int IdEstadoCaso { get; set; }
    public int? IdUsuarioAsignado { get; set; }
    public string Canal { get; set; } = null!;
    public string Asunto { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaResolucion { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public TipoCaso TipoCaso { get; set; } = null!;
    public EstadoCaso EstadoCaso { get; set; } = null!;
    public Usuario? UsuarioAsignado { get; set; }
}
