namespace ShopEasy.BW.Campanas;

public class CrearCampanaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdCanal { get; set; }
    public DateTime FechaProgramada { get; set; }
    public string? Estado { get; set; }
    public string Contenido { get; set; } = string.Empty;
    public List<int>? IdClientes { get; set; }
}

public class ActualizarCampanaRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int IdCanal { get; set; }
    public DateTime FechaProgramada { get; set; }
    public string Contenido { get; set; } = string.Empty;
}

public class CambiarEstadoCampanaRequest
{
    public string Estado { get; set; } = string.Empty;
}

public class AsociarClientesCampanaRequest
{
    public List<int> IdClientes { get; set; } = new();
}

public class ActualizarEnvioCampanaRequest
{
    public string EstadoEnvio { get; set; } = string.Empty;
    public DateTime? FechaEnvio { get; set; }
}

public record CanalMarketingDto(
    int IdCanal,
    string Nombre,
    string? Descripcion,
    bool Activo);

public record CampanaClienteDto(
    int IdCampanaCliente,
    int IdCliente,
    string Cliente,
    string? Correo,
    string EstadoEnvio,
    DateTime? FechaEnvio);

public record CampanaResponse(
    int IdCampana,
    string Nombre,
    string? Descripcion,
    int IdCanal,
    string Canal,
    DateTime FechaProgramada,
    string Estado,
    DateTime FechaCreacion,
    int IdUsuarioCreador,
    string Creador);

public record CampanaDetalleResponse(
    int IdCampana,
    string Nombre,
    string? Descripcion,
    int IdCanal,
    string Canal,
    DateTime FechaProgramada,
    string Estado,
    string Contenido,
    DateTime FechaCreacion,
    int IdUsuarioCreador,
    string Creador,
    List<CampanaClienteDto> Clientes);
