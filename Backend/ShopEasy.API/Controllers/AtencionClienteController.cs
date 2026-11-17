using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Services;
using ShopEasy.BW.AtencionCliente;
using ShopEasy.BW.Common;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/atencion-cliente")]
[Authorize]
public class AtencionClienteController : ControllerBase
{
    private readonly AtencionClienteService _atencionClienteService;

    public AtencionClienteController(AtencionClienteService atencionClienteService)
    {
        _atencionClienteService = atencionClienteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CasoClienteResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CasoClienteResponse>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] int? idCliente,
        [FromQuery] int? idEstadoCaso,
        [FromQuery] int? idTipoCaso,
        [FromQuery] int? idUsuarioAsignado,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _atencionClienteService.ObtenerCasosAsync(
            buscar,
            idCliente,
            idEstadoCaso,
            idTipoCaso,
            idUsuarioAsignado,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("tipos")]
    [ProducesResponseType(typeof(List<TipoCasoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TipoCasoDto>>> ObtenerTipos()
    {
        return Ok(await _atencionClienteService.ObtenerTiposAsync());
    }

    [HttpGet("estados")]
    [ProducesResponseType(typeof(List<EstadoCasoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<EstadoCasoDto>>> ObtenerEstados()
    {
        return Ok(await _atencionClienteService.ObtenerEstadosAsync());
    }

    [HttpGet("usuarios")]
    [ProducesResponseType(typeof(List<UsuarioAsignableDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UsuarioAsignableDto>>> ObtenerUsuarios()
    {
        return Ok(await _atencionClienteService.ObtenerUsuariosAsignablesAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CasoClienteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CasoClienteResponse>> ObtenerPorId(int id)
    {
        return Ok(await _atencionClienteService.ObtenerCasoAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CasoClienteResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<CasoClienteResponse>> Crear(
        [FromBody] CrearCasoRequest request)
    {
        var caso = await _atencionClienteService.CrearCasoAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = caso.IdCaso }, caso);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CasoClienteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CasoClienteResponse>> Actualizar(
        int id,
        [FromBody] ActualizarCasoRequest request)
    {
        return Ok(await _atencionClienteService.ActualizarCasoAsync(id, request));
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(CasoClienteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CasoClienteResponse>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoCasoRequest request)
    {
        return Ok(await _atencionClienteService.CambiarEstadoAsync(id, request));
    }

    [HttpPatch("{id:int}/asignacion")]
    [ProducesResponseType(typeof(CasoClienteResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CasoClienteResponse>> Asignar(
        int id,
        [FromBody] AsignarCasoRequest request)
    {
        return Ok(await _atencionClienteService.AsignarCasoAsync(id, request));
    }
}
