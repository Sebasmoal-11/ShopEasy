using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Common;
using ShopEasy.BC.Services;
using ShopEasy.BW.Campanas;
using ShopEasy.BW.Common;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/campanas")]
[Authorize]
public class CampanasController : ControllerBase
{
    private readonly CampanasService _campanasService;

    public CampanasController(CampanasService campanasService)
    {
        _campanasService = campanasService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CampanaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CampanaResponse>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] string? estado,
        [FromQuery] int? idCanal,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _campanasService.ObtenerCampanasAsync(
            buscar,
            estado,
            idCanal,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("canales")]
    [ProducesResponseType(typeof(List<CanalMarketingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CanalMarketingDto>>> ObtenerCanales()
    {
        return Ok(await _campanasService.ObtenerCanalesAsync());
    }

    [HttpGet("estados")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<string>>> ObtenerEstados()
    {
        return Ok(await _campanasService.ObtenerEstadosAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CampanaDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanaDetalleResponse>> ObtenerPorId(int id)
    {
        return Ok(await _campanasService.ObtenerCampanaAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CampanaDetalleResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CampanaDetalleResponse>> Crear(
        [FromBody] CrearCampanaRequest request)
    {
        var campana = await _campanasService.CrearCampanaAsync(
            request,
            ObtenerUsuarioId());

        return CreatedAtAction(nameof(ObtenerPorId), new { id = campana.IdCampana }, campana);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CampanaDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanaDetalleResponse>> Actualizar(
        int id,
        [FromBody] ActualizarCampanaRequest request)
    {
        return Ok(await _campanasService.ActualizarCampanaAsync(id, request));
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(CampanaDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanaDetalleResponse>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoCampanaRequest request)
    {
        return Ok(await _campanasService.CambiarEstadoAsync(id, request));
    }

    [HttpGet("{id:int}/clientes")]
    [ProducesResponseType(typeof(List<CampanaClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CampanaClienteDto>>> ObtenerClientes(int id)
    {
        return Ok(await _campanasService.ObtenerClientesAsync(id));
    }

    [HttpPost("{id:int}/clientes")]
    [ProducesResponseType(typeof(CampanaDetalleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanaDetalleResponse>> AsociarClientes(
        int id,
        [FromBody] AsociarClientesCampanaRequest request)
    {
        return Ok(await _campanasService.AsociarClientesAsync(id, request));
    }

    [HttpPatch("{id:int}/clientes/{idCliente:int}/envio")]
    [ProducesResponseType(typeof(CampanaClienteDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CampanaClienteDto>> ActualizarEnvio(
        int id,
        int idCliente,
        [FromBody] ActualizarEnvioCampanaRequest request)
    {
        return Ok(await _campanasService.ActualizarEnvioAsync(
            id,
            idCliente,
            request));
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
