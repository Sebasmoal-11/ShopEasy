using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Common;
using ShopEasy.BC.Services;
using ShopEasy.BW.Common;
using ShopEasy.BW.Productos;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/productos")]
[Authorize]
public class ProductosController : ControllerBase
{
    private readonly ProductosService _productosService;

    public ProductosController(ProductosService productosService)
    {
        _productosService = productosService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<ProductoResponse>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] bool? activo,
        [FromQuery] int? idBodega,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _productosService.ObtenerProductosAsync(
            buscar,
            activo,
            idBodega,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("bodegas")]
    [ProducesResponseType(typeof(List<BodegaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<BodegaDto>>> ObtenerBodegas()
    {
        return Ok(await _productosService.ObtenerBodegasAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductoResponse>> ObtenerPorId(int id)
    {
        return Ok(await _productosService.ObtenerProductoAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductoResponse>> Crear(
        [FromBody] CrearProductoRequest request)
    {
        var producto = await _productosService.CrearProductoAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.IdProducto }, producto);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductoResponse>> Actualizar(
        int id,
        [FromBody] ActualizarProductoRequest request)
    {
        return Ok(await _productosService.ActualizarProductoAsync(id, request));
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(ProductoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductoResponse>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoProductoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "El estado es obligatorio.");
        }

        return Ok(await _productosService.CambiarEstadoAsync(id, request.Activo));
    }
}
