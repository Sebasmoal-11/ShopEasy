namespace ShopEasy.BW.Users;

public class CreateUserRequest
{
    public string Cedula { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Apellidos { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public int IdRol { get; set; }
}
