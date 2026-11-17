using Microsoft.EntityFrameworkCore;
using ShopEasy.DA.Entities;

namespace ShopEasy.DA.Data;

public class ProductosDA
{
    private readonly ShopEasyDbContext _context;

    public ProductosDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductoVendido>> ObtenerMasVendidosAsync(int limite)
    {
        var filas = await (
                from detalle in _context.DetallePedido.AsNoTracking()
                join producto in _context.Productos.AsNoTracking()
                    on detalle.IdProducto equals producto.IdProducto
                join pedido in _context.Pedidos.AsNoTracking()
                    on detalle.IdPedido equals pedido.IdPedido
                group detalle by new { producto.IdProducto, producto.Nombre }
                into grupo
                orderby grupo.Sum(detalle => detalle.Cantidad) descending,
                    grupo.Sum(detalle => detalle.Cantidad * detalle.PrecioUnitario) descending
                select new
                {
                    grupo.Key.IdProducto,
                    grupo.Key.Nombre,
                    CantidadVendida = grupo.Sum(detalle => detalle.Cantidad),
                    MontoGenerado = grupo.Sum(detalle => detalle.Cantidad * detalle.PrecioUnitario)
                })
            .Take(limite)
            .ToListAsync();

        return filas
            .Select(fila => new ProductoVendido(
                fila.IdProducto,
                fila.Nombre,
                fila.CantidadVendida,
                Math.Round(fila.MontoGenerado, 2)))
            .ToList();
    }

    public async Task<PagedResult<Producto>> ObtenerListadoAsync(
        string? buscar,
        bool? activo,
        int? idBodega,
        int pagina,
        int tamano)
    {
        var consulta = _context.Productos
            .AsNoTracking()
            .Include(producto => producto.Bodega)
            .AsQueryable();

        if (activo.HasValue)
        {
            consulta = consulta.Where(producto => producto.Activo == activo.Value);
        }

        if (idBodega.HasValue)
        {
            consulta = consulta.Where(producto => producto.IdBodega == idBodega.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var termino = buscar.Trim();

            consulta = consulta.Where(producto =>
                producto.Nombre.Contains(termino) ||
                (producto.Descripcion != null &&
                 producto.Descripcion.Contains(termino)));
        }

        var total = await consulta.CountAsync();

        var items = await consulta
            .OrderBy(producto => producto.Nombre)
            .ThenBy(producto => producto.IdProducto)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync();

        return new PagedResult<Producto>(total, items);
    }

    public Task<Producto?> ObtenerDetalleAsync(int idProducto)
    {
        return _context.Productos
            .AsNoTracking()
            .Include(producto => producto.Bodega)
            .FirstOrDefaultAsync(producto => producto.IdProducto == idProducto);
    }

    public Task<Producto?> ObtenerParaEdicionAsync(int idProducto)
    {
        return _context.Productos
            .Include(producto => producto.Bodega)
            .FirstOrDefaultAsync(producto => producto.IdProducto == idProducto);
    }

    public Task<List<Producto>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids)
    {
        return _context.Productos
            .AsNoTracking()
            .Where(producto => ids.Contains(producto.IdProducto))
            .ToListAsync();
    }

    public Task<List<Bodega>> ObtenerBodegasAsync()
    {
        return _context.Bodegas
            .AsNoTracking()
            .OrderBy(bodega => bodega.Nombre)
            .ToListAsync();
    }

    public Task<Bodega?> ObtenerBodegaAsync(int idBodega)
    {
        return _context.Bodegas
            .AsNoTracking()
            .FirstOrDefaultAsync(bodega => bodega.IdBodega == idBodega);
    }

    public async Task CrearAsync(Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
    }

    public Task ActualizarAsync(Producto producto)
    {
        return _context.SaveChangesAsync();
    }
}
