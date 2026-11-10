namespace ShopEasy.BW.Users;

public record UserResponse(
    int IdUsuario,
    string Cedula,
    string NombreCompleto,
    string Correo,
    int IdRol,
    string Rol,
    bool Activo,
    DateTime FechaCreacion);
