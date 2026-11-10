namespace ShopEasy.BW.Auth;

public record CurrentUserDto(
    int IdUsuario,
    string Cedula,
    string NombreCompleto,
    string Correo,
    string Rol);
