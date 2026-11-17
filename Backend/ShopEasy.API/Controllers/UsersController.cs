using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Common;
using ShopEasy.BC.Services;
using ShopEasy.BW.Common;
using ShopEasy.BW.Users;

namespace ShopEasy.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Administrador")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<UserResponse>>> Obtener(
        [FromQuery] string? buscar,
        [FromQuery] bool? activo,
        [FromQuery] int? idRol,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20)
    {
        var respuesta = await _userService.ObtenerUsuariosAsync(
            buscar,
            activo,
            idRol,
            pagina,
            tamano);

        return Ok(respuesta);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> ObtenerPorId(int id)
    {
        return Ok(await _userService.ObtenerUsuarioAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] CreateUserRequest request)
    {
        var usuario = await _userService.CrearUsuarioAsync(request);
        return StatusCode(StatusCodes.Status201Created, usuario);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> Actualizar(
        int id,
        [FromBody] UpdateUserRequest request)
    {
        return Ok(await _userService.ActualizarUsuarioAsync(
            id,
            request,
            ObtenerUsuarioId()));
    }

    [HttpPatch("{id:int}/estado")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserResponse>> CambiarEstado(
        int id,
        [FromBody] CambiarEstadoUsuarioRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "El estado es obligatorio.");
        }

        return Ok(await _userService.CambiarEstadoAsync(
            id,
            request.Activo,
            ObtenerUsuarioId()));
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<RoleResponse>>> ObtenerRoles()
    {
        var roles = await _userService.ObtenerRolesAsync();
        return Ok(roles);
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
