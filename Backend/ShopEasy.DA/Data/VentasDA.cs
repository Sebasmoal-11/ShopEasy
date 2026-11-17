using Microsoft.EntityFrameworkCore;

namespace ShopEasy.DA.Data;

public class VentasDA
{
    private readonly ShopEasyDbContext _context;

    public VentasDA(ShopEasyDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> SumarVentasEntreAsync(DateTime desde, DateTime hastaExclusivo)
    {
        var total = await _context.Pedidos
            .AsNoTracking()
            .Where(pedido => pedido.FechaPedido >= desde && pedido.FechaPedido < hastaExclusivo)
            .SumAsync(pedido => (decimal?)pedido.Total);

        return total ?? 0m;
    }

    public async Task<List<ConteoPedidoPorDia>> ObtenerVentasPorDiaAsync(
        DateTime desde,
        DateTime hastaExclusivo)
    {
        var filas = await _context.Pedidos
            .AsNoTracking()
            .Where(pedido => pedido.FechaPedido >= desde && pedido.FechaPedido < hastaExclusivo)
            .GroupBy(pedido => new
            {
                pedido.FechaPedido.Year,
                pedido.FechaPedido.Month,
                pedido.FechaPedido.Day
            })
            .Select(grupo => new
            {
                grupo.Key.Year,
                grupo.Key.Month,
                grupo.Key.Day,
                CantidadPedidos = grupo.Count(),
                MontoTotal = grupo.Sum(pedido => pedido.Total)
            })
            .OrderBy(fila => fila.Year)
            .ThenBy(fila => fila.Month)
            .ThenBy(fila => fila.Day)
            .ToListAsync();

        return filas
            .Select(fila => new ConteoPedidoPorDia(
                new DateTime(fila.Year, fila.Month, fila.Day),
                fila.CantidadPedidos,
                fila.MontoTotal))
            .ToList();
    }
}
