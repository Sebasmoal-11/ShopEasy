using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class AuthDA
{
    private readonly ShopEasyDbContext _context;

    public AuthDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> BuscarPorCorreoAsync(string correo)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(usuario => usuario.Rol)
            .FirstOrDefaultAsync(usuario =>
                usuario.Persona.CorreosPersona.Any(c => c.Correo.ToLower() == correo));
    }
}
