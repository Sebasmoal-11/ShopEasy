namespace ShopEasy.BW.Users;

public class UpdateUserRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public string? Contrasena { get; set; }
}

public class CambiarEstadoUsuarioRequest
{
    public bool Activo { get; set; }
}
