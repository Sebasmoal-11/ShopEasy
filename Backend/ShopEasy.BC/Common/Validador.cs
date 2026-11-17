using System.ComponentModel.DataAnnotations;

namespace ShopEasy.BC.Common;

public static class Validador
{
    public static string Texto(string? valor, string campo, int longitudMaxima, int longitudMinima = 1)
    {
        var texto = (valor ?? string.Empty).Trim();

        if (texto.Length < longitudMinima)
        {
            throw new ApiException(400, $"{campo} es obligatorio.");
        }

        if (texto.Length > longitudMaxima)
        {
            throw new ApiException(400, $"{campo} no puede superar {longitudMaxima} caracteres.");
        }

        return texto;
    }

    public static string? TextoOpcional(string? valor, string campo, int longitudMaxima)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var texto = valor.Trim();

        if (texto.Length > longitudMaxima)
        {
            throw new ApiException(400, $"{campo} no puede superar {longitudMaxima} caracteres.");
        }

        return texto;
    }

    public static string Correo(string? valor)
    {
        var correo = (valor ?? string.Empty).Trim().ToLowerInvariant();

        if (correo.Length > 150 || !new EmailAddressAttribute().IsValid(correo))
        {
            throw new ApiException(400, "El correo electronico no es valido.");
        }

        return correo;
    }

    public static string? CorreoOpcional(string? valor)
    {
        return string.IsNullOrWhiteSpace(valor) ? null : Correo(valor);
    }

    public static (int Pagina, int Tamano) Paginacion(int pagina, int tamano)
    {
        if (pagina < 1)
        {
            throw new ApiException(400, "La pagina debe ser mayor o igual a 1.");
        }

        if (tamano is < 1 or > 100)
        {
            throw new ApiException(400, "El tamano de pagina debe estar entre 1 y 100.");
        }

        return (pagina, tamano);
    }

    public static void Precio(decimal precio)
    {
        if (precio <= 0 || precio > 99999999.99m)
        {
            throw new ApiException(400, "El precio debe ser mayor que cero y menor que 100000000.");
        }
    }

    public static void Stock(int stock)
    {
        if (stock < 0)
        {
            throw new ApiException(400, "El stock no puede ser negativo.");
        }
    }

    public static void Fecha(DateTime fecha, string campo)
    {
        if (fecha == default)
        {
            throw new ApiException(400, $"{campo} es obligatoria.");
        }
    }
}
