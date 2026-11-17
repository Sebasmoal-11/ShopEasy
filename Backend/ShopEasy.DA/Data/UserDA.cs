using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class UserDA
{
    private readonly ShopEasyDbContext _context;

    public UserDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteCedulaAsync(string cedula)
    {
        return _context.Personas.AnyAsync(persona => persona.Cedula == cedula);
    }

    public Task<bool> ExisteCorreoAsync(string correo)
    {
        return _context.CorreosPersona.AnyAsync(c =>
            c.Correo.ToLower() == correo);
    }

    public Task<bool> ExisteCorreoEnOtraPersonaAsync(string correo, string cedula)
    {
        return _context.CorreosPersona.AnyAsync(c =>
            c.Correo.ToLower() == correo && c.Cedula != cedula);
    }

    public Task<Rol?> ObtenerRolAsync(int idRol)
    {
        return _context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(rol => rol.IdRol == idRol);
    }

    public Task<List<Rol>> ObtenerRolesAsync()
    {
        return _context.Roles
            .AsNoTracking()
            .OrderBy(rol => rol.Nombre)
            .ToListAsync();
    }

    public async Task<PagedResult<Usuario>> ObtenerListadoAsync(
        string? buscar,
        bool? activo,
        int? idRol,
        int pagina,
        int tamano)
    {
        var consulta = _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(usuario => usuario.Rol)
            .AsQueryable();

        if (activo.HasValue)
        {
            consulta = consulta.Where(usuario => usuario.Activo == activo.Value);
        }

        if (idRol.HasValue)
        {
            consulta = consulta.Where(usuario => usuario.IdRol == idRol.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(usuario =>
                usuario.Cedula.Contains(termino) ||
                usuario.Persona.Nombre.Contains(termino) ||
                usuario.Persona.Apellidos.Contains(termino) ||
                usuario.Persona.CorreosPersona.Any(correo =>
                    correo.Correo.Contains(termino)));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderBy(usuario => usuario.Persona.Apellidos)
            .ThenBy(usuario => usuario.Persona.Nombre)
            .ThenBy(usuario => usuario.IdUsuario)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<Usuario>(total, items);
    }

    public Task<Usuario?> ObtenerDetalleAsync(int idUsuario)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(usuario => usuario.Rol)
            .FirstOrDefaultAsync(usuario => usuario.IdUsuario == idUsuario);
    }

    public Task<Usuario?> ObtenerParaEdicionAsync(int idUsuario)
    {
        return _context.Usuarios
            .Include(usuario => usuario.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(usuario => usuario.Rol)
            .FirstOrDefaultAsync(usuario => usuario.IdUsuario == idUsuario);
    }

    public Task<Usuario?> ObtenerActivoAsync(int idUsuario)
    {
        return _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(usuario =>
                usuario.IdUsuario == idUsuario && usuario.Activo);
    }

    public Task<int> ContarAdministradoresActivosAsync()
    {
        return _context.Usuarios
            .AsNoTracking()
            .CountAsync(usuario =>
                usuario.Activo && usuario.Rol.Nombre == "Administrador");
    }

    public Task<int> ContarAdministradoresActivosExcluyendoAsync(int idUsuario)
    {
        return _context.Usuarios
            .AsNoTracking()
            .CountAsync(usuario =>
                usuario.Activo &&
                usuario.IdUsuario != idUsuario &&
                usuario.Rol.Nombre == "Administrador");
    }

    public async Task CrearUsuarioAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }

    public Task ActualizarAsync(Usuario usuario)
    {
        return _context.SaveChangesAsync();
    }
}
