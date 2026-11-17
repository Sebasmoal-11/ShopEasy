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
    private readonly VentasService _ventasService;
    private readonly PedidosService _pedidosService;
    private readonly ProductosService _productosService;
    private readonly ClientesService _clientesService;
    private readonly AtencionClienteService _atencionClienteService;
    private readonly CampanasService _campanasService;

    public DashboardController(
        DashboardService dashboardService,
        VentasService ventasService,
        PedidosService pedidosService,
        ProductosService productosService,
        ClientesService clientesService,
        AtencionClienteService atencionClienteService,
        CampanasService campanasService)
    {
        _dashboardService = dashboardService;
        _ventasService = ventasService;
        _pedidosService = pedidosService;
        _productosService = productosService;
        _clientesService = clientesService;
        _atencionClienteService = atencionClienteService;
        _campanasService = campanasService;
    }

    /// <summary>
    /// Resumen general: usuario autenticado y tarjetas principales del Dashboard.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(DashboardResponse), StatusCodes.Status200OK)]
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

    /// <summary>
    /// Ventas de los ultimos N dias (por defecto 30), con serie diaria y acumulados de 7 y 30 dias.
    /// </summary>
    [HttpGet("ventas")]
    [ProducesResponseType(typeof(VentasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<VentasResponse>> ObtenerVentas([FromQuery] int dias = 30)
    {
        var respuesta = await _ventasService.ObtenerVentasAsync(dias);
        return Ok(respuesta);
    }

    /// <summary>
    /// Cantidad de pedidos agrupados por cada estado existente en la base de datos.
    /// </summary>
    [HttpGet("pedidos")]
    [ProducesResponseType(typeof(PedidosEstadosResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidosEstadosResponse>> ObtenerPedidos()
    {
        var respuesta = await _pedidosService.ObtenerEstadosAsync();
        return Ok(respuesta);
    }

    /// <summary>
    /// Productos mas vendidos por cantidad, con monto total generado.
    /// </summary>
    [HttpGet("productos-mas-vendidos")]
    [ProducesResponseType(typeof(ProductosMasVendidosResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductosMasVendidosResponse>> ObtenerProductosMasVendidos(
        [FromQuery] int limite = 5)
    {
        var respuesta = await _productosService.ObtenerMasVendidosAsync(limite);
        return Ok(respuesta);
    }

    /// <summary>
    /// Clientes con mayor cantidad de pedidos y monto total de compras.
    /// </summary>
    [HttpGet("clientes-destacados")]
    [ProducesResponseType(typeof(ClientesDestacadosResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClientesDestacadosResponse>> ObtenerClientesDestacados(
        [FromQuery] int limite = 5)
    {
        var respuesta = await _clientesService.ObtenerDestacadosAsync(limite);
        return Ok(respuesta);
    }

    /// <summary>
    /// Estadisticas de atencion al cliente: totales, tipos y canales.
    /// </summary>
    [HttpGet("atencion-cliente")]
    [ProducesResponseType(typeof(AtencionClienteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AtencionClienteResponse>> ObtenerAtencionCliente()
    {
        var respuesta = await _atencionClienteService.ObtenerEstadisticasAsync();
        return Ok(respuesta);
    }

    /// <summary>
    /// Estadisticas de campanas: totales por estado y proximas campanas programadas.
    /// </summary>
    [HttpGet("campanas")]
    [ProducesResponseType(typeof(CampanasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanasResponse>> ObtenerCampanas([FromQuery] int limite = 5)
    {
        var respuesta = await _campanasService.ObtenerEstadisticasAsync(limite);
        return Ok(respuesta);
    }
}
