namespace ShopEasy.DA.Data;

public record CreacionPedidoResultado(
    bool Exito,
    int? IdPedido,
    int? IdProductoSinStock);

public record UsuarioAsignable(
    int IdUsuario,
    string NombreCompleto,
    string Rol,
    bool Activo);
