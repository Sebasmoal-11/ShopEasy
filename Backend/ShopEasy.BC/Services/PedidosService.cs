using ShopEasy.BC.Common;
using ShopEasy.BW.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.BW.Pedidos;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class PedidosService
{
    private const int MaximoDetalles = 50;

    private readonly PedidosDA _pedidosDA;
    private readonly ProductosDA _productosDA;
    private readonly ClientesDA _clientesDA;

    public PedidosService(
        PedidosDA pedidosDA,
        ProductosDA productosDA,
        ClientesDA clientesDA)
    {
        _pedidosDA = pedidosDA;
        _productosDA = productosDA;
        _clientesDA = clientesDA;
    }

    public async Task<PedidosEstadosResponse> ObtenerEstadosAsync()
    {
        var estados = await _pedidosDA.ObtenerConteosPorEstadoAsync();

        return new PedidosEstadosResponse(
            estados.Sum(estado => estado.Cantidad),
            estados
                .Select(estado => new PedidoEstadoDto(
                    estado.IdEstadoPedido,
                    estado.Estado,
                    estado.Cantidad))
                .ToList());
    }

    public async Task<List<EstadoPedidoDto>> ObtenerCatalogoEstadosAsync()
    {
        var estados = await _pedidosDA.ObtenerEstadosAsync();

        return estados
            .Select(estado => new EstadoPedidoDto(
                estado.IdEstadoPedido,
                estado.Nombre,
                estado.Descripcion))
            .ToList();
    }

    public async Task<PagedResponse<PedidoResumenDto>> ObtenerPedidosAsync(
        string? buscar,
        int? idCliente,
        int? idEstadoPedido,
        DateTime? desde,
        DateTime? hasta,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var desdeNormalizado = desde?.Date;
        var hastaExclusivo = hasta?.Date.AddDays(1);

        if (desdeNormalizado.HasValue &&
            hastaExclusivo.HasValue &&
            desdeNormalizado.Value >= hastaExclusivo.Value)
        {
            throw new ApiException(400, "La fecha inicial no puede ser posterior a la fecha final.");
        }

        var resultado = await _pedidosDA.ObtenerListadoAsync(
            buscar,
            idCliente,
            idEstadoPedido,
            desdeNormalizado,
            hastaExclusivo,
            pagina,
            tamano);

        return new PagedResponse<PedidoResumenDto>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items
                .Select(PedidoMapper.MapearResumen)
                .ToList());
    }

    public async Task<PedidoDetalleResponse> ObtenerPedidoAsync(int idPedido)
    {
        var pedido = await _pedidosDA.ObtenerDetalleAsync(idPedido);

        if (pedido is null)
        {
            throw new ApiException(404, "El pedido no existe.");
        }

        return PedidoMapper.MapearDetalle(pedido);
    }

    public async Task<List<PedidoResumenDto>> ObtenerPorClienteAsync(int idCliente)
    {
        var pedidos = await _pedidosDA.ObtenerPorClienteAsync(idCliente);

        return pedidos
            .Select(PedidoMapper.MapearResumen)
            .ToList();
    }

    public async Task<PedidoDetalleResponse> CrearPedidoAsync(
        CrearPedidoRequest request,
        int idUsuario)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del pedido son obligatorios.");
        }

        if (request.IdCliente <= 0)
        {
            throw new ApiException(400, "Debe seleccionar un cliente.");
        }

        var direccion = Validador.Texto(
            request.DireccionEnvio,
            "La direccion de envio",
            255,
            5);

        var observaciones = Validador.TextoOpcional(
            request.Observaciones,
            "Las observaciones",
            2000);

        if (request.Detalles is null || request.Detalles.Count == 0)
        {
            throw new ApiException(400, "El pedido debe incluir al menos un producto.");
        }

        if (request.Detalles.Count > MaximoDetalles)
        {
            throw new ApiException(400, $"El pedido no puede tener mas de {MaximoDetalles} lineas.");
        }

        var cantidades = new Dictionary<int, long>();

        foreach (var detalle in request.Detalles)
        {
            if (detalle.IdProducto <= 0)
            {
                throw new ApiException(400, "Cada detalle debe indicar un producto valido.");
            }

            if (detalle.Cantidad <= 0)
            {
                throw new ApiException(400, "La cantidad de cada producto debe ser mayor que cero.");
            }

            cantidades.TryGetValue(detalle.IdProducto, out var acumulada);
            cantidades[detalle.IdProducto] = acumulada + detalle.Cantidad;
        }

        if (cantidades.Values.Any(cantidad => cantidad > int.MaxValue))
        {
            throw new ApiException(400, "La cantidad solicitada es demasiado grande.");
        }

        var cliente = await _clientesDA.ObtenerActivoAsync(request.IdCliente);

        if (cliente is null)
        {
            throw new ApiException(400, "El cliente no existe o esta inactivo.");
        }

        var idsProductos = cantidades.Keys.ToList();
        var productos = await _productosDA.ObtenerPorIdsAsync(idsProductos);
        var mapaProductos = productos.ToDictionary(producto => producto.IdProducto);

        var estado = await _pedidosDA.ObtenerEstadoInicialAsync();

        if (estado is null)
        {
            throw new ApiException(400, "No hay estados de pedido configurados en la base de datos.");
        }

        var detalles = new List<DetallePedido>();
        var total = 0m;

        foreach (var (idProducto, cantidad) in cantidades)
        {
            if (!mapaProductos.TryGetValue(idProducto, out var producto))
            {
                throw new ApiException(400, $"El producto {idProducto} no existe.");
            }

            if (!producto.Activo)
            {
                throw new ApiException(400, $"El producto \"{producto.Nombre}\" esta inactivo.");
            }

            if (producto.Stock < cantidad)
            {
                throw new ApiException(
                    409,
                    $"Stock insuficiente para \"{producto.Nombre}\". Disponible: {producto.Stock}.");
            }

            var cantidadEntera = (int)cantidad;
            var subtotal = Math.Round(producto.Precio * cantidadEntera, 2);
            total = Math.Round(total + subtotal, 2);

            detalles.Add(new DetallePedido
            {
                IdProducto = producto.IdProducto,
                Cantidad = cantidadEntera,
                PrecioUnitario = producto.Precio
            });
        }

        if (total > 99999999.99m)
        {
            throw new ApiException(400, "El total del pedido excede el maximo permitido.");
        }

        var pedido = new Pedido
        {
            IdCliente = request.IdCliente,
            IdEstadoPedido = estado.IdEstadoPedido,
            FechaPedido = DateTime.Now,
            Total = total,
            DireccionEnvio = direccion,
            Observaciones = observaciones
        };

        pedido.DetallePedido = detalles;

        var creacion = await _pedidosDA.CrearPedidoAsync(
            pedido,
            detalles,
            idUsuario);

        if (!creacion.Exito)
        {
            var nombreProducto = creacion.IdProductoSinStock.HasValue &&
                mapaProductos.TryGetValue(creacion.IdProductoSinStock.Value, out var afectado)
                    ? afectado.Nombre
                    : "seleccionado";

            throw new ApiException(
                409,
                $"Stock insuficiente para \"{nombreProducto}\" al confirmar el pedido.");
        }

        return await ObtenerPedidoAsync(creacion.IdPedido!.Value);
    }

    public async Task<PedidoDetalleResponse> CambiarEstadoAsync(
        int idPedido,
        ActualizarEstadoPedidoRequest request,
        int idUsuario)
    {
        if (request is null || request.IdEstadoPedido <= 0)
        {
            throw new ApiException(400, "Debe seleccionar un estado valido.");
        }

        var estado = await _pedidosDA.ObtenerEstadoAsync(request.IdEstadoPedido);

        if (estado is null)
        {
            throw new ApiException(400, "El estado de pedido seleccionado no existe.");
        }

        var observacion = Validador.TextoOpcional(
            request.Observacion,
            "La observacion",
            255);

        var actual = await ObtenerPedidoAsync(idPedido);

        if (actual.IdEstadoPedido == estado.IdEstadoPedido)
        {
            return actual;
        }

        var actualizado = await _pedidosDA.ActualizarEstadoAsync(
            idPedido,
            estado.IdEstadoPedido,
            idUsuario,
            observacion);

        if (!actualizado)
        {
            throw new ApiException(404, "El pedido no existe.");
        }

        return await ObtenerPedidoAsync(idPedido);
    }
}
