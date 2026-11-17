using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Common;
using ShopEasy.BC.Services;
using ShopEasy.BW.Common;
using ShopEasy.BW.Pedidos;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize]
public class PedidosController : ControllerBase
{
    private readonly PedidosService _pedidosService;

    public PedidosController(PedidosService pedidosService)
    {
        _pedidosService = pedidosService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PedidoResumenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PedidoResumenDto>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] int? idCliente,
        [FromQuery] int? idEstadoPedido,
        [FromQuery] DateTime? desde,
        [FromQuery] DateTime? hasta,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _pedidosService.ObtenerPedidosAsync(
            buscar,
            idCliente,
            idEstadoPedido,
            desde,
            hasta,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("estados")]
    [ProducesResponseType(typeof(List<EstadoPedidoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EstadoPedidoDto>>> ObtenerEstados()
    {
        return Ok(await _pedidosService.ObtenerCatalogoEstadosAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDetalleResponse>> ObtenerPorId(int id)
    {
        return Ok(await _pedidosService.ObtenerPedidoAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PedidoDetalleResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<PedidoDetalleResponse>> Crear(
        [FromBody] CrearPedidoRequest request)
    {
        var pedido = await _pedidosService.CrearPedidoAsync(
            request,
            ObtenerUsuarioId());

        return CreatedAtAction(nameof(ObtenerPorId), new { id = pedido.IdPedido }, pedido);
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(PedidoDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDetalleResponse>> CambiarEstado(
        int id,
        [FromBody] ActualizarEstadoPedidoRequest request)
    {
        return Ok(await _pedidosService.CambiarEstadoAsync(
            id,
            request,
            ObtenerUsuarioId()));
    }

    private int ObtenerUsuarioId()
    {
        var idTexto = User.FindFirstValue("sub");

        if (!int.TryParse(idTexto, out var idUsuario))
        {
            throw new ApiException(401, "La sesion ya no es valida.");
        }

        return idUsuario;
    }
}
