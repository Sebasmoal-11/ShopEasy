namespace ShopEasy.DA.Entities;

/// <summary>
/// Correo electronico asociado a una persona (tabla <c>correos_persona</c>).
/// </summary>
public class CorreoPersona
{
    public int IdCorreo { get; set; }
    public string Cedula { get; set; } = null!;
    public string Correo { get; set; } = null!;
    public string TipoCorreo { get; set; } = null!;
    public bool Principal { get; set; }
    public bool Activo { get; set; }

    public Persona Persona { get; set; } = null!;
}
