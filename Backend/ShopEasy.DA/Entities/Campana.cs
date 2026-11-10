namespace ShopEasy.DA.Entities;

/// <summary>
/// Campana de marketing (tabla <c>campanas</c>).
/// </summary>
public class Campana
{
    public int IdCampana { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public int IdCanal { get; set; }
    public DateTime FechaProgramada { get; set; }
    public string Estado { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }
    public int IdUsuarioCreador { get; set; }

    public CanalMarketing Canal { get; set; } = null!;
    public Usuario UsuarioCreador { get; set; } = null!;

    public ICollection<CampanaCliente> CampanaClientes { get; set; } = new List<CampanaCliente>();
    public ICollection<CampanaInteres> CampanaIntereses { get; set; } = new List<CampanaInteres>();
}
