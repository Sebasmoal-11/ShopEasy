namespace ShopEasy.BW.Dashboard;

public record CampanasResponse(
    int TotalCampanas,
    int CampanasBorrador,
    int CampanasProgramadas,
    int CampanasEnviadasOFinalizadas,
    List<CampanaPorEstadoDto> PorEstado,
    List<ProximaCampanaDto> ProximasCampanas);

public record CampanaPorEstadoDto(
    string Estado,
    int Cantidad);

public record ProximaCampanaDto(
    int IdCampana,
    string Nombre,
    string Canal,
    DateTime FechaProgramada,
    string Estado);
