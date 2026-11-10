namespace ShopEasy.DA.Entities;

/// <summary>
/// Telefono asociado a una persona (tabla <c>telefonos_persona</c>).
/// </summary>
public class TelefonoPersona
{
    public int IdTelefono { get; set; }
    public string Cedula { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string TipoTelefono { get; set; } = null!;
    public bool Principal { get; set; }
    public bool Activo { get; set; }

    public Persona Persona { get; set; } = null!;
}
