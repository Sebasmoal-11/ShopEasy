using System.ComponentModel.DataAnnotations;
using ShopEasy.BC.Common;
using ShopEasy.BC.Security;
using ShopEasy.BW.Users;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class UserService
{
    private readonly UserDA _userDA;

    public UserService(UserDA userDA)
    {
        _userDA = userDA;
    }

    public async Task<UserResponse> CrearUsuarioAsync(CreateUserRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos de la solicitud son obligatorios.");
        }

        var cedula = (request.Cedula ?? string.Empty).Trim();
        var nombre = (request.Nombre ?? string.Empty).Trim();
        var apellidos = (request.Apellidos ?? string.Empty).Trim();
        var correo = (request.Correo ?? string.Empty).Trim().ToLowerInvariant();
        var contrasena = request.Contrasena ?? string.Empty;

        ValidarDatos(cedula, nombre, apellidos, correo, contrasena, request.IdRol);

        var rol = await _userDA.ObtenerRolAsync(request.IdRol);

        if (rol is null)
        {
            throw new ApiException(400, "El rol seleccionado no existe.");
        }

        if (await _userDA.ExisteCedulaAsync(cedula))
        {
            throw new ApiException(409, "Ya existe una persona registrada con esa cedula.");
        }

        if (await _userDA.ExisteCorreoAsync(correo))
        {
            throw new ApiException(409, "El correo electronico ya esta registrado.");
        }

        var fecha = DateTime.Now;

        var persona = new Persona
        {
            Cedula = cedula,
            Nombre = nombre,
            Apellidos = apellidos,
            FechaCreacion = fecha
        };

        var correoPersona = new CorreoPersona
        {
            Cedula = cedula,
            Correo = correo,
            TipoCorreo = "Personal",
            Principal = true,
            Activo = true,
            Persona = persona
        };

        persona.CorreosPersona.Add(correoPersona);

        var usuario = new Usuario
        {
            Cedula = cedula,
            IdRol = rol.IdRol,
            ContrasenaHash = PasswordHelper.Hash(contrasena),
            Activo = true,
            FechaCreacion = fecha,
            Persona = persona
        };

        await _userDA.CrearUsuarioAsync(usuario);

        return new UserResponse(
            usuario.IdUsuario,
            persona.Cedula,
            $"{persona.Nombre} {persona.Apellidos}".Trim(),
            correoPersona.Correo,
            rol.IdRol,
            rol.Nombre,
            usuario.Activo,
            usuario.FechaCreacion);
    }

    public async Task<List<RoleResponse>> ObtenerRolesAsync()
    {
        var roles = await _userDA.ObtenerRolesAsync();

        return roles
            .Select(rol => new RoleResponse(rol.IdRol, rol.Nombre, rol.Descripcion))
            .ToList();
    }

    private static void ValidarDatos(
        string cedula,
        string nombre,
        string apellidos,
        string correo,
        string contrasena,
        int idRol)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(cedula))
        {
            errores.Add("La cedula es obligatoria.");
        }
        else if (cedula.Length > 30)
        {
            errores.Add("La cedula no puede superar 30 caracteres.");
        }

        if (nombre.Length is < 2 or > 100)
        {
            errores.Add("El nombre debe tener entre 2 y 100 caracteres.");
        }

        if (apellidos.Length is < 2 or > 100)
        {
            errores.Add("Los apellidos deben tener entre 2 y 100 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(correo) ||
            correo.Length > 150 ||
            !new EmailAddressAttribute().IsValid(correo))
        {
            errores.Add("El correo electronico no es valido.");
        }

        errores.AddRange(PasswordHelper.Validate(contrasena));

        if (idRol <= 0)
        {
            errores.Add("Debe seleccionar un rol.");
        }

        if (errores.Count > 0)
        {
            throw new ApiException(400, string.Join(" ", errores));
        }
    }
}
