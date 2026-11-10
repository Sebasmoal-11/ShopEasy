namespace ShopEasy.DA.Entities;

/// <summary>
/// Usuario que puede iniciar sesion en el CRM (tabla <c>usuarios</c>).
/// </summary>
public class Usuario
{
    public int IdUsuario { get; set; }
    public string Cedula { get; set; } = null!;
    public int IdRol { get; set; }
    public string ContrasenaHash { get; set; } = null!;
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Persona Persona { get; set; } = null!;
    public Rol Rol { get; set; } = null!;

    public ICollection<HistorialEstadoPedido> HistorialEstadosPedido { get; set; } = new List<HistorialEstadoPedido>();
    public ICollection<CasoCliente> CasosClienteAsignados { get; set; } = new List<CasoCliente>();
    public ICollection<Campana> CampanasCreadas { get; set; } = new List<Campana>();
}
