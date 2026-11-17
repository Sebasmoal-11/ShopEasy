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

    public Task<int> ContarClientesActivosAsync()
    {
        return _context.Clientes
            .AsNoTracking()
            .CountAsync(cliente => cliente.Activo);
    }

    public Task<int> ContarPedidosAsync()
    {
        return _context.Pedidos.CountAsync();
    }

    public async Task<decimal> SumarVentasAsync()
    {
        var total = await _context.Pedidos
            .AsNoTracking()
            .SumAsync(pedido => (decimal?)pedido.Total);

        return total ?? 0m;
    }

    public Task<int> ContarCasosAsync()
    {
        return _context.CasosCliente.CountAsync();
    }

    public Task<int> ContarCampanasAsync()
    {
        return _context.Campanas.CountAsync();
    }

    public Task<int> ContarProductosActivosAsync()
    {
        return _context.Productos
            .AsNoTracking()
            .CountAsync(producto => producto.Activo);
    }
}
