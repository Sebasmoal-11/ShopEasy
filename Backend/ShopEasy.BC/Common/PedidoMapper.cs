using ShopEasy.BW.Pedidos;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Common;

public static class PedidoMapper
{
    public static PedidoResumenDto MapearResumen(Pedido pedido)
    {
        return new PedidoResumenDto(
            pedido.IdPedido,
            pedido.IdCliente,
            ContactosHelper.NombreCompleto(pedido.Cliente.Persona),
            pedido.Cliente.Cedula,
            pedido.IdEstadoPedido,
            pedido.EstadoPedido.Nombre,
            pedido.FechaPedido,
            pedido.Total,
            pedido.DireccionEnvio,
            pedido.DetallePedido.Sum(detalle => detalle.Cantidad));
    }

    public static PedidoDetalleResponse MapearDetalle(Pedido pedido)
    {
        var detalles = pedido.DetallePedido
            .OrderBy(detalle => detalle.IdDetalle)
            .Select(detalle => new PedidoItemDto(
                detalle.IdProducto,
                detalle.Producto.Nombre,
                detalle.Cantidad,
                detalle.PrecioUnitario,
                Math.Round(detalle.Cantidad * detalle.PrecioUnitario, 2)))
            .ToList();

        var historial = pedido.HistorialEstadosPedido
            .OrderBy(cambio => cambio.FechaCambio)
            .ThenBy(cambio => cambio.IdHistorial)
            .Select(cambio => new HistorialEstadoPedidoDto(
                cambio.IdHistorial,
                cambio.IdEstadoPedido,
                cambio.EstadoPedido.Nombre,
                cambio.FechaCambio,
                cambio.IdUsuario,
                cambio.Usuario is null
                    ? null
                    : ContactosHelper.NombreCompleto(cambio.Usuario.Persona),
                cambio.Observacion))
            .ToList();

        return new PedidoDetalleResponse(
            pedido.IdPedido,
            pedido.IdCliente,
            ContactosHelper.NombreCompleto(pedido.Cliente.Persona),
            pedido.Cliente.Cedula,
            NullIfEmpty(ContactosHelper.ObtenerCorreoPrincipal(pedido.Cliente.Persona)),
            pedido.IdEstadoPedido,
            pedido.EstadoPedido.Nombre,
            pedido.FechaPedido,
            pedido.Total,
            pedido.DireccionEnvio,
            pedido.Observaciones,
            detalles,
            historial);
    }

    private static string? NullIfEmpty(string valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : valor;
    }
}
