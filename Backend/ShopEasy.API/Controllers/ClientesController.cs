using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Common;
using ShopEasy.BC.Services;
using ShopEasy.BW.Clientes;
using ShopEasy.BW.Common;
using ShopEasy.BW.Pedidos;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/clientes")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly ClientesService _clientesService;

    public ClientesController(ClientesService clientesService)
    {
        _clientesService = clientesService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ClienteResponse>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] bool? activo,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _clientesService.ObtenerClientesAsync(
            buscar,
            activo,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("preferencias-publicidad")]
    [ProducesResponseType(typeof(List<PreferenciaPublicidadDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PreferenciaPublicidadDto>>> ObtenerPreferencias()
    {
        return Ok(await _clientesService.ObtenerPreferenciasAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClienteDetalleResponse>> ObtenerPorId(int id)
    {
        return Ok(await _clientesService.ObtenerClienteAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDetalleResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ClienteDetalleResponse>> Crear(
        [FromBody] CrearClienteRequest request)
    {
        var cliente = await _clientesService.CrearClienteAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.IdCliente }, cliente);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ClienteDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClienteDetalleResponse>> Actualizar(
        int id,
        [FromBody] ActualizarClienteRequest request)
    {
        return Ok(await _clientesService.ActualizarClienteAsync(id, request));
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(ClienteDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClienteDetalleResponse>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoClienteRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "El estado es obligatorio.");
        }

        return Ok(await _clientesService.CambiarEstadoAsync(id, request.Activo));
    }

    [HttpGet("{id:int}/historial-compras")]
    [ProducesResponseType(typeof(ClienteComprasResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ClienteComprasResponse>> ObtenerHistorialCompras(int id)
    {
        return Ok(await _clientesService.ObtenerComprasAsync(id));
    }
}
