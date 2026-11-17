using ShopEasy.BC.Common;
using ShopEasy.BC.Security;
using ShopEasy.BW.Common;
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

    public async Task<PagedResponse<UserResponse>> ObtenerUsuariosAsync(
        string? buscar,
        bool? activo,
        int? idRol,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var resultado = await _userDA.ObtenerListadoAsync(
            buscar,
            activo,
            idRol,
            pagina,
            tamano);

        return new PagedResponse<UserResponse>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items.Select(usuario => Mapear(usuario)).ToList());
    }

    public async Task<UserResponse> ObtenerUsuarioAsync(int idUsuario)
    {
        var usuario = await _userDA.ObtenerDetalleAsync(idUsuario);

        if (usuario is null)
        {
            throw new ApiException(404, "El usuario no existe.");
        }

        return Mapear(usuario);
    }

    public async Task<UserResponse> ActualizarUsuarioAsync(
        int idUsuario,
        UpdateUserRequest request,
        int idUsuarioActual)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del usuario son obligatorios.");
        }

        var usuario = await _userDA.ObtenerParaEdicionAsync(idUsuario);

        if (usuario is null)
        {
            throw new ApiException(404, "El usuario no existe.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre", 100, 2);
        var apellidos = Validador.Texto(request.Apellidos, "Los apellidos", 100, 2);
        var correo = Validador.Correo(request.Correo);

        if (request.IdRol <= 0)
        {
            throw new ApiException(400, "Debe seleccionar un rol.");
        }

        var rol = await _userDA.ObtenerRolAsync(request.IdRol);

        if (rol is null)
        {
            throw new ApiException(400, "El rol seleccionado no existe.");
        }

        if (await _userDA.ExisteCorreoEnOtraPersonaAsync(correo, usuario.Cedula))
        {
            throw new ApiException(409, "El correo electronico ya esta registrado por otra persona.");
        }

        var eraAdministrador = usuario.Rol.Nombre == "Administrador";
        var seguiraSiendoAdministrador = rol.Nombre == "Administrador";

        if (usuario.Activo &&
            eraAdministrador &&
            !seguiraSiendoAdministrador &&
            await _userDA.ContarAdministradoresActivosExcluyendoAsync(idUsuario) == 0)
        {
            throw new ApiException(400, "No se puede quitar el rol al ultimo administrador activo.");
        }

        usuario.Persona.Nombre = nombre;
        usuario.Persona.Apellidos = apellidos;
        usuario.IdRol = rol.IdRol;

        var correoActual = usuario.Persona.CorreosPersona
            .Where(c => c.Activo)
            .OrderByDescending(c => c.Principal)
            .ThenBy(c => c.IdCorreo)
            .FirstOrDefault();

        if (correoActual is null)
        {
            usuario.Persona.CorreosPersona.Add(new CorreoPersona
            {
                Cedula = usuario.Cedula,
                Correo = correo,
                TipoCorreo = "Personal",
                Principal = true,
                Activo = true
            });
        }
        else
        {
            if (usuario.Persona.CorreosPersona.Any(c =>
                    c.IdCorreo != correoActual.IdCorreo &&
                    c.Correo.Equals(correo, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ApiException(409, "Ese correo ya esta asociado al usuario.");
            }

            correoActual.Correo = correo;
            correoActual.Principal = true;
            correoActual.Activo = true;
        }

        if (!string.IsNullOrWhiteSpace(request.Contrasena))
        {
            var errores = PasswordHelper.Validate(request.Contrasena);

            if (errores.Count > 0)
            {
                throw new ApiException(400, string.Join(" ", errores));
            }

            usuario.ContrasenaHash = PasswordHelper.Hash(request.Contrasena);
        }

        await _userDA.ActualizarAsync(usuario);

        return await ObtenerUsuarioAsync(idUsuario);
    }

    public async Task<UserResponse> CambiarEstadoAsync(
        int idUsuario,
        bool activo,
        int idUsuarioActual)
    {
        var usuario = await _userDA.ObtenerParaEdicionAsync(idUsuario);

        if (usuario is null)
        {
            throw new ApiException(404, "El usuario no existe.");
        }

        if (!activo && idUsuario == idUsuarioActual)
        {
            throw new ApiException(400, "No puedes desactivar tu propio usuario.");
        }

        if (!activo &&
            usuario.Activo &&
            usuario.Rol.Nombre == "Administrador" &&
            await _userDA.ContarAdministradoresActivosExcluyendoAsync(idUsuario) == 0)
        {
            throw new ApiException(400, "No se puede desactivar al ultimo administrador activo.");
        }

        usuario.Activo = activo;
        await _userDA.ActualizarAsync(usuario);

        return await ObtenerUsuarioAsync(idUsuario);
    }

    public async Task<List<RoleResponse>> ObtenerRolesAsync()
    {
        var roles = await _userDA.ObtenerRolesAsync();

        return roles
            .Select(rol => new RoleResponse(rol.IdRol, rol.Nombre, rol.Descripcion))
            .ToList();
    }

    private static UserResponse Mapear(Usuario usuario, string? correo = null)
    {
        var correoPrincipal = correo ??
            ContactosHelper.ObtenerCorreoPrincipal(usuario.Persona);

        return new UserResponse(
            usuario.IdUsuario,
            usuario.Cedula,
            ContactosHelper.NombreCompleto(usuario.Persona),
            correoPrincipal,
            usuario.IdRol,
            usuario.Rol.Nombre,
            usuario.Activo,
            usuario.FechaCreacion);
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
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(correo))
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
