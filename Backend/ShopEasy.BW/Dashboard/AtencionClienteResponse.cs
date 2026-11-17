namespace ShopEasy.BW.Dashboard;

public record AtencionClienteResponse(
    int TotalCasos,
    int CasosPendientes,
    int CasosEnProceso,
    int CasosResueltos,
    List<CasoPorEstadoDto> PorEstado,
    List<CasoPorTipoDto> PorTipo,
    List<CasoPorCanalDto> PorCanal);

public record CasoPorEstadoDto(
    int IdEstadoCaso,
    string Estado,
    int Cantidad);

public record CasoPorTipoDto(
    int IdTipoCaso,
    string Tipo,
    int Cantidad);

public record CasoPorCanalDto(
    string Canal,
    int Cantidad);
