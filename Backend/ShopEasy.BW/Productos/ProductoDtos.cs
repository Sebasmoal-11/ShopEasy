namespace ShopEasy.BW.Productos;

public record BodegaDto(
    int IdBodega,
    string Nombre,
    string Direccion,
    string? Descripcion,
    bool Activo);

public record ProductoResponse(
    int IdProducto,
    int IdBodega,
    string Bodega,
    string Nombre,
    string? Descripcion,
    decimal Precio,
    int Stock,
    bool Activo,
    DateTime FechaCreacion);

public class CrearProductoRequest
{
    public int IdBodega { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class ActualizarProductoRequest
{
    public int IdBodega { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
}

public class CambiarEstadoProductoRequest
{
    public bool Activo { get; set; }
}
