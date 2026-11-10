namespace ShopEasy.DA.Entities;

/// <summary>
/// Persona fisica o juridica registrada en el CRM (tabla <c>personas</c>).
/// </summary>
public class Persona
{
    public string Cedula { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }

    public ICollection<CorreoPersona> CorreosPersona { get; set; } = new List<CorreoPersona>();
    public ICollection<TelefonoPersona> TelefonosPersona { get; set; } = new List<TelefonoPersona>();

    // La cedula es unica en clientes y en usuarios: la relacion real es 1:1.
    public Cliente? Cliente { get; set; }
    public Usuario? Usuario { get; set; }
}
