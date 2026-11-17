using ShopEasy.BC.Common;
using ShopEasy.BW.Common;
using ShopEasy.BW.Dashboard;
using ShopEasy.BW.Productos;
using ShopEasy.DA.Data;
using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Services;

public class ProductosService
{
    private const int LimiteMinimo = 1;
    private const int LimiteMaximo = 20;

    private readonly ProductosDA _productosDA;

    public ProductosService(ProductosDA productosDA)
    {
        _productosDA = productosDA;
    }

    public async Task<ProductosMasVendidosResponse> ObtenerMasVendidosAsync(int limite)
    {
        if (limite < LimiteMinimo || limite > LimiteMaximo)
        {
            throw new ApiException(400, "El limite debe estar entre 1 y 20.");
        }

        var productos = await _productosDA.ObtenerMasVendidosAsync(limite);

        return new ProductosMasVendidosResponse(
            productos
                .Select(producto => new ProductoMasVendidoDto(
                    producto.IdProducto,
                    producto.Nombre,
                    producto.CantidadVendida,
                    producto.MontoGenerado))
                .ToList());
    }

    public async Task<PagedResponse<ProductoResponse>> ObtenerProductosAsync(
        string? buscar,
        bool? activo,
        int? idBodega,
        int pagina,
        int tamano)
    {
        (pagina, tamano) = Validador.Paginacion(pagina, tamano);

        var resultado = await _productosDA.ObtenerListadoAsync(
            buscar,
            activo,
            idBodega,
            pagina,
            tamano);

        return new PagedResponse<ProductoResponse>(
            resultado.Total,
            pagina,
            tamano,
            resultado.Items.Select(Mapear).ToList());
    }

    public async Task<ProductoResponse> ObtenerProductoAsync(int idProducto)
    {
        var producto = await _productosDA.ObtenerDetalleAsync(idProducto);

        if (producto is null)
        {
            throw new ApiException(404, "El producto no existe.");
        }

        return Mapear(producto);
    }

    public async Task<ProductoResponse> CrearProductoAsync(CrearProductoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del producto son obligatorios.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre del producto", 150, 2);
        var descripcion = Validador.TextoOpcional(request.Descripcion, "La descripcion", 4000);

        Validador.Precio(request.Precio);
        Validador.Stock(request.Stock);

        var bodega = await _productosDA.ObtenerBodegaAsync(request.IdBodega);

        if (bodega is null || !bodega.Activo)
        {
            throw new ApiException(400, "La bodega seleccionada no existe o esta inactiva.");
        }

        var producto = new Producto
        {
            IdBodega = bodega.IdBodega,
            Nombre = nombre,
            Descripcion = descripcion,
            Precio = request.Precio,
            Stock = request.Stock,
            Activo = true,
            FechaCreacion = DateTime.Now
        };

        await _productosDA.CrearAsync(producto);

        return await ObtenerProductoAsync(producto.IdProducto);
    }

    public async Task<ProductoResponse> ActualizarProductoAsync(
        int idProducto,
        ActualizarProductoRequest request)
    {
        if (request is null)
        {
            throw new ApiException(400, "Los datos del producto son obligatorios.");
        }

        var producto = await _productosDA.ObtenerParaEdicionAsync(idProducto);

        if (producto is null)
        {
            throw new ApiException(404, "El producto no existe.");
        }

        var nombre = Validador.Texto(request.Nombre, "El nombre del producto", 150, 2);
        var descripcion = Validador.TextoOpcional(request.Descripcion, "La descripcion", 4000);

        Validador.Precio(request.Precio);
        Validador.Stock(request.Stock);

        var bodega = await _productosDA.ObtenerBodegaAsync(request.IdBodega);

        if (bodega is null || !bodega.Activo)
        {
            throw new ApiException(400, "La bodega seleccionada no existe o esta inactiva.");
        }

        producto.IdBodega = bodega.IdBodega;
        producto.Nombre = nombre;
        producto.Descripcion = descripcion;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;

        await _productosDA.ActualizarAsync(producto);

        return await ObtenerProductoAsync(idProducto);
    }

    public async Task<ProductoResponse> CambiarEstadoAsync(int idProducto, bool activo)
    {
        var producto = await _productosDA.ObtenerParaEdicionAsync(idProducto);

        if (producto is null)
        {
            throw new ApiException(404, "El producto no existe.");
        }

        producto.Activo = activo;
        await _productosDA.ActualizarAsync(producto);

        return await ObtenerProductoAsync(idProducto);
    }

    public async Task<List<BodegaDto>> ObtenerBodegasAsync()
    {
        var bodegas = await _productosDA.ObtenerBodegasAsync();

        return bodegas
            .Select(bodega => new BodegaDto(
                bodega.IdBodega,
                bodega.Nombre,
                bodega.Direccion,
                bodega.Descripcion,
                bodega.Activo))
            .ToList();
    }

    private static ProductoResponse Mapear(Producto producto)
    {
        return new ProductoResponse(
            producto.IdProducto,
            producto.IdBodega,
            producto.Bodega?.Nombre ?? string.Empty,
            producto.Nombre,
            producto.Descripcion,
            producto.Precio,
            producto.Stock,
            producto.Activo,
            producto.FechaCreacion);
    }
}
