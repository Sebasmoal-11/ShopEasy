using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Services;
using ShopEasy.BW.Dashboard;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Obtener()
    {
        var idTexto = User.FindFirstValue("sub");

        if (!int.TryParse(idTexto, out var idUsuario))
        {
            return Unauthorized();
        }

        var dashboard = await _dashboardService.ObtenerDashboardAsync(idUsuario);
        return Ok(dashboard);
    }
}
