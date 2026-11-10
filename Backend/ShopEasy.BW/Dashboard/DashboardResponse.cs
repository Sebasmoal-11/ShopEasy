using ShopEasy.BW.Auth;

namespace ShopEasy.BW.Dashboard;

public record DashboardResponse(
    DateTime GeneratedAtUtc,
    CurrentUserDto User,
    DashboardStatsDto Stats);

public record DashboardStatsDto(
    int TotalClientes,
    int TotalPedidos,
    int TotalCasosCliente,
    int TotalCampanas);
