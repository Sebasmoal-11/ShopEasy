namespace ShopEasy.BW.Dashboard;

public record ClientesDestacadosResponse(
    List<ClienteDestacadoDto> Clientes);

public record ClienteDestacadoDto(
    int IdCliente,
    string NombreCompleto,
    string? Correo,
    int CantidadPedidos,
    decimal MontoTotal);
