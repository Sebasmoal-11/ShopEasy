using BCrypt.Net;

namespace ShopEasy.BC.Security;

public static class PasswordHelper
{
    private const int WorkFactor = 12;

    public static string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public static bool Verify(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static List<string> Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("La contrasena es obligatoria.");
            return errors;
        }

        if (password.Length < 10)
        {
            errors.Add("La contrasena debe tener al menos 10 caracteres.");
        }

        if (password.Length > 72)
        {
            errors.Add("La contrasena no puede superar 72 caracteres.");
        }

        if (!password.Any(char.IsUpper))
        {
            errors.Add("La contrasena debe incluir una letra mayuscula.");
        }

        if (!password.Any(char.IsLower))
        {
            errors.Add("La contrasena debe incluir una letra minuscula.");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("La contrasena debe incluir un numero.");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            errors.Add("La contrasena debe incluir un simbolo.");
        }

        return errors;
    }
}
