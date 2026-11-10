using ShopEasy.BC.Common;
using ShopEasy.BW.Auth;
using ShopEasy.BW.Dashboard;
using ShopEasy.DA.Data;

namespace ShopEasy.BC.Services;

public class DashboardService
{
    private readonly DashboardDA _dashboardDA;

    public DashboardService(DashboardDA dashboardDA)
    {
        _dashboardDA = dashboardDA;
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

        var totalClientes = await _dashboardDA.ContarClientesAsync();
        var totalPedidos = await _dashboardDA.ContarPedidosAsync();
        var totalCasos = await _dashboardDA.ContarCasosAsync();
        var totalCampanas = await _dashboardDA.ContarCampanasAsync();

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
                totalCampanas));
    }
}
