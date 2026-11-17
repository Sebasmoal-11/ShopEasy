using ShopEasy.DA.Entities;

namespace ShopEasy.BC.Common;

public static class ContactosHelper
{
    public static string ObtenerCorreoPrincipal(Persona? persona)
    {
        return persona?.CorreosPersona
            .Where(correo => correo.Activo)
            .OrderByDescending(correo => correo.Principal)
            .ThenBy(correo => correo.IdCorreo)
            .Select(correo => correo.Correo)
            .FirstOrDefault() ?? string.Empty;
    }

    public static string? ObtenerTelefonoPrincipal(Persona? persona)
    {
        return persona?.TelefonosPersona
            .Where(telefono => telefono.Activo)
            .OrderByDescending(telefono => telefono.Principal)
            .ThenBy(telefono => telefono.IdTelefono)
            .Select(telefono => telefono.Telefono)
            .FirstOrDefault();
    }

    public static string NombreCompleto(Persona persona)
    {
        return $"{persona.Nombre} {persona.Apellidos}".Trim();
    }
}
