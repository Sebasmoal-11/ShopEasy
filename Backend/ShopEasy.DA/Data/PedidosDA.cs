using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class PedidosDA
{
    private readonly ShopEasyDbContext _context;

    public PedidosDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ConteoPedidoEstado>> ObtenerConteosPorEstadoAsync()
    {
        var estados = await _context.EstadosPedido
            .AsNoTracking()
            .OrderBy(estado => estado.Nombre)
            .Select(estado => new { estado.IdEstadoPedido, estado.Nombre })
            .ToListAsync();

        var conteos = await _context.Pedidos
            .AsNoTracking()
            .GroupBy(pedido => pedido.IdEstadoPedido)
            .Select(grupo => new
            {
                IdEstadoPedido = grupo.Key,
                Cantidad = grupo.Count()
            })
            .ToListAsync();

        var mapa = conteos.ToDictionary(conteo => conteo.IdEstadoPedido, conteo => conteo.Cantidad);

        return estados
            .Select(estado => new ConteoPedidoEstado(
                estado.IdEstadoPedido,
                estado.Nombre,
                mapa.GetValueOrDefault(estado.IdEstadoPedido)))
            .ToList();
    }

    public Task<List<EstadoPedido>> ObtenerEstadosAsync()
    {
        return _context.EstadosPedido
            .AsNoTracking()
            .OrderBy(estado => estado.Nombre)
            .ToListAsync();
    }

    public Task<EstadoPedido?> ObtenerEstadoAsync(int idEstadoPedido)
    {
        return _context.EstadosPedido
            .AsNoTracking()
            .FirstOrDefaultAsync(estado => estado.IdEstadoPedido == idEstadoPedido);
    }

    public async Task<EstadoPedido?> ObtenerEstadoInicialAsync()
    {
        var pendiente = await _context.EstadosPedido
            .AsNoTracking()
            .Where(estado => estado.Nombre.Contains("Pendient"))
            .OrderBy(estado => estado.IdEstadoPedido)
            .FirstOrDefaultAsync();

        return pendiente ?? await _context.EstadosPedido
            .AsNoTracking()
            .OrderBy(estado => estado.IdEstadoPedido)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<Pedido>> ObtenerListadoAsync(
        string? buscar,
        int? idCliente,
        int? idEstadoPedido,
        DateTime? desde,
        DateTime? hastaExclusivo,
        int pagina,
        int tamano)
    {
        var consulta = _context.Pedidos
            .AsNoTracking()
            .Include(pedido => pedido.Cliente)
                .ThenInclude(cliente => cliente.Persona)
            .Include(pedido => pedido.EstadoPedido)
            .Include(pedido => pedido.DetallePedido)
            .AsQueryable();

        if (idCliente.HasValue)
        {
            consulta = consulta.Where(pedido => pedido.IdCliente == idCliente.Value);
        }

        if (idEstadoPedido.HasValue)
        {
            consulta = consulta.Where(pedido =>
                pedido.IdEstadoPedido == idEstadoPedido.Value);
        }

        if (desde.HasValue)
        {
            consulta = consulta.Where(pedido => pedido.FechaPedido >= desde.Value);
        }

        if (hastaExclusivo.HasValue)
        {
            consulta = consulta.Where(pedido => pedido.FechaPedido < hastaExclusivo.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(pedido =>
                pedido.Cliente.Cedula.Contains(termino) ||
                pedido.Cliente.Persona.Nombre.Contains(termino) ||
                pedido.Cliente.Persona.Apellidos.Contains(termino));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderByDescending(pedido => pedido.FechaPedido)
            .ThenByDescending(pedido => pedido.IdPedido)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<Pedido>(total, items);
    }

    public Task<Pedido?> ObtenerDetalleAsync(int idPedido)
    {
        return _context.Pedidos
            .AsNoTracking()
            .Include(pedido => pedido.Cliente)
                .ThenInclude(cliente => cliente.Persona)
                    .ThenInclude(persona => persona.CorreosPersona)
            .Include(pedido => pedido.EstadoPedido)
            .Include(pedido => pedido.DetallePedido)
                .ThenInclude(detalle => detalle.Producto)
            .Include(pedido => pedido.HistorialEstadosPedido)
                .ThenInclude(historial => historial.EstadoPedido)
            .Include(pedido => pedido.HistorialEstadosPedido)
                .ThenInclude(historial => historial.Usuario)
                    .ThenInclude(usuario => usuario!.Persona)
            .FirstOrDefaultAsync(pedido => pedido.IdPedido == idPedido);
    }

    public Task<List<Pedido>> ObtenerPorClienteAsync(int idCliente)
    {
        return _context.Pedidos
            .AsNoTracking()
            .Include(pedido => pedido.Cliente)
                .ThenInclude(cliente => cliente.Persona)
            .Include(pedido => pedido.EstadoPedido)
            .Include(pedido => pedido.DetallePedido)
            .Where(pedido => pedido.IdCliente == idCliente)
            .OrderByDescending(pedido => pedido.FechaPedido)
            .ThenByDescending(pedido => pedido.IdPedido)
            .ToListAsync();
    }

    public async Task<CreacionPedidoResultado> CrearPedidoAsync(
        Pedido pedido,
        IReadOnlyCollection<DetallePedido> detalles,
        int idUsuario)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        foreach (var detalle in detalles)
        {
            var afectadas = await _context.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE productos
                SET stock = stock - {detalle.Cantidad}
                WHERE id_producto = {detalle.IdProducto}
                  AND activo = 1
                  AND stock >= {detalle.Cantidad}
                """);

            if (afectadas == 0)
            {
                await transaccion.RollbackAsync();
                return new CreacionPedidoResultado(false, null, detalle.IdProducto);
            }
        }

        pedido.HistorialEstadosPedido.Add(new HistorialEstadoPedido
        {
            IdEstadoPedido = pedido.IdEstadoPedido,
            IdUsuario = idUsuario,
            FechaCambio = DateTime.Now,
            Observacion = "Pedido registrado"
        });

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();
        await transaccion.CommitAsync();

        return new CreacionPedidoResultado(true, pedido.IdPedido, null);
    }

    public async Task<bool> ActualizarEstadoAsync(
        int idPedido,
        int idEstadoPedido,
        int idUsuario,
        string? observacion)
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(pedido => pedido.IdPedido == idPedido);

        if (pedido is null)
        {
            return false;
        }

        pedido.IdEstadoPedido = idEstadoPedido;

        _context.HistorialEstadosPedido.Add(new HistorialEstadoPedido
        {
            IdPedido = idPedido,
            IdEstadoPedido = idEstadoPedido,
            IdUsuario = idUsuario,
            FechaCambio = DateTime.Now,
            Observacion = string.IsNullOrWhiteSpace(observacion)
                ? null
                : observacion.Trim()
        });

        await _context.SaveChangesAsync();
        return true;
    }
}
