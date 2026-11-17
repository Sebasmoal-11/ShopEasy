using ShopEasy.BC.Common;
using ShopEasy.BW.Auth;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;

namespace ShopEasy.BC.Services;

public class DashboardService
{
    private readonly DashboardDA _dashboardDA;
    private readonly AtencionClienteDA _atencionClienteDA;

    public DashboardService(DashboardDA dashboardDA, AtencionClienteDA atencionClienteDA)
    {
        _dashboardDA = dashboardDA;
        _atencionClienteDA = atencionClienteDA;
    }

    public async Task<DashboardResponse> ObtenerDashboardAsync(int idUsuario)
    {
        var usuario = await _dashboardDA.ObtenerUsuarioAsync(idUsuario);

        if (usuario is null)
        {
            throw new ApiException(401, "La sesion ya no es valida.");
        }

        var correo = usuario.Persona.CorreosPersona
            .Where(c => c.Activo)
            .OrderByDescending(c => c.Principal)
            .ThenBy(c => c.IdCorreo)
            .Select(c => c.Correo)
            .FirstOrDefault() ?? string.Empty;

        var totalClientes = await _dashboardDA.ContarClientesActivosAsync();
        var totalPedidos = await _dashboardDA.ContarPedidosAsync();
        var totalVentas = await _dashboardDA.SumarVentasAsync();
        var totalCasos = await _dashboardDA.ContarCasosAsync();
        var totalProductos = await _dashboardDA.ContarProductosActivosAsync();
        var totalCampanas = await _dashboardDA.ContarCampanasAsync();

        var casosPorEstado = await _atencionClienteDA.ObtenerConteosPorEstadoAsync();
        var totalCasosPendientes = casosPorEstado
            .Where(caso => ClasificadorEstados.EsCasoPendiente(caso.Estado))
            .Sum(caso => caso.Cantidad);

        return new DashboardResponse(
            DateTime.UtcNow,
            new CurrentUserDto(
                usuario.IdUsuario,
                usuario.Cedula,
                $"{usuario.Persona.Nombre} {usuario.Persona.Apellidos}".Trim(),
                correo,
                usuario.Rol.Nombre),
            new DashboardStatsDto(
                totalClientes,
                totalPedidos,
                totalCasos,
                totalCampanas,
                totalVentas,
                totalCasosPendientes,
                totalProductos));
    }
}
