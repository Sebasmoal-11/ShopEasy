namespace ShopEasy.BW.AtencionCliente;

public class CrearCasoRequest
{
    public int IdCliente { get; set; }
    public int IdTipoCaso { get; set; }
    public int? IdUsuarioAsignado { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class ActualizarCasoRequest
{
    public int IdTipoCaso { get; set; }
    public int? IdUsuarioAsignado { get; set; }
    public string Canal { get; set; } = string.Empty;
    public string Asunto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class CambiarEstadoCasoRequest
{
    public int IdEstadoCaso { get; set; }
}

public class AsignarCasoRequest
{
    public int? IdUsuarioAsignado { get; set; }
}

public record TipoCasoDto(
    int IdTipoCaso,
    string Nombre,
    string? Descripcion);

public record EstadoCasoDto(
    int IdEstadoCaso,
    string Nombre,
    string? Descripcion);

public record UsuarioAsignableDto(
    int IdUsuario,
    string NombreCompleto,
    string Rol,
    bool Activo);

public record CasoClienteResponse(
    int IdCaso,
    int IdCliente,
    string Cliente,
    string Cedula,
    int IdTipoCaso,
    string Tipo,
    int IdEstadoCaso,
    string Estado,
    int? IdUsuarioAsignado,
    string? UsuarioAsignado,
    string Canal,
    string Asunto,
    string Descripcion,
    DateTime FechaCreacion,
    DateTime? FechaResolucion);
