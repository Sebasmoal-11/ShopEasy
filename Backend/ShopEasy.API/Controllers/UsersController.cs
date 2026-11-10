using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopEasy.BC.Services;
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

    [HttpPost]
    public async Task<IActionResult> Crear(CreateUserRequest request)
    {
        var usuario = await _userService.CrearUsuarioAsync(request);
        return StatusCode(StatusCodes.Status201Created, usuario);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<RoleResponse>>> ObtenerRoles()
    {
        var roles = await _userService.ObtenerRolesAsync();
        return Ok(roles);
    }
}
