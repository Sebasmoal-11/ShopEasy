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

    public async Task CrearUsuarioAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
    }
}
