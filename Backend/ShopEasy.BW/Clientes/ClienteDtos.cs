using ShopEasy.BW.Pedidos;

namespace ShopEasy.BW.Clientes;

public record CorreoPersonaDto(
    int IdCorreo,
    string Correo,
    string TipoCorreo,
    bool Principal,
    bool Activo);

public record TelefonoPersonaDto(
    int IdTelefono,
    string Telefono,
    string TipoTelefono,
    bool Principal,
    bool Activo);

public record PreferenciaPublicidadDto(
    int IdPreferenciaPublicidad,
    string Nombre,
    string? Descripcion);

public record ClienteResponse(
    int IdCliente,
    string Cedula,
    string Nombre,
    string Apellidos,
    string NombreCompleto,
    string? CorreoPrincipal,
    string? TelefonoPrincipal,
    int IdPreferenciaPublicidad,
    string PreferenciaPublicidad,
    bool Activo,
    DateTime FechaRegistro);

public record ClienteDetalleResponse(
    int IdCliente,
    string Cedula,
    string Nombre,
    string Apellidos,
    string NombreCompleto,
    int IdPreferenciaPublicidad,
    string PreferenciaPublicidad,
    bool Activo,
    DateTime FechaRegistro,
    List<CorreoPersonaDto> Correos,
    List<TelefonoPersonaDto> Telefonos);

public record ClienteComprasResponse(
    ClienteResponse Cliente,
    int TotalPedidos,
    decimal TotalComprado,
    List<PedidoResumenDto> Compras);

public class CrearClienteRequest
{
    public string Cedula { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? TipoCorreo { get; set; }
    public string? TipoTelefono { get; set; }
    public int? IdPreferenciaPublicidad { get; set; }
}

public class ActualizarClienteRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? TipoCorreo { get; set; }
    public string? TipoTelefono { get; set; }
    public int? IdPreferenciaPublicidad { get; set; }
}

public class CambiarEstadoClienteRequest
{
    public bool Activo { get; set; }
}
