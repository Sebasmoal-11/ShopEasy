using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class DashboardDA
{
    private readonly ShopEasyDbContext _context;

    public DashboardDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public Task<Usuario?> ObtenerUsuarioAsync(int idUsuario)
    {
        return _context.Usuarios
            .AsNoTracking()
            .Include(usuario => usuario.Persona)
                .ThenInclude(persona => persona.CorreosPersona)
            .Include(usuario => usuario.Rol)
            .FirstOrDefaultAsync(usuario => usuario.IdUsuario == idUsuario);
    }

    public Task<int> ContarClientesAsync()
    {
        return _context.Clientes.CountAsync();
    }

    public Task<int> ContarPedidosAsync()
    {
        return _context.Pedidos.CountAsync();
    }

    public Task<int> ContarCasosAsync()
    {
        return _context.CasosCliente.CountAsync();
    }

    public Task<int> ContarCampanasAsync()
    {
        return _context.Campanas.CountAsync();
    }
}
