namespace ShopEasy.BC.Common;

/// <summary>
/// Interpreta los nombres de estado configurados en MySQL sin depender de
/// identificadores fijos. Los nombres se comparan por fragmento y sin
/// distinguir mayusculas.
/// </summary>
public static class ClasificadorEstados
{
    public static bool EsCasoPendiente(string estado)
    {
        return Coincide(estado, "pendient", "abiert");
    }

    public static bool EsCasoEnProceso(string estado)
    {
        return Coincide(estado, "proceso", "progreso");
    }

    public static bool EsCasoResuelto(string estado)
    {
        return Coincide(estado, "resuelt", "cerrad");
    }

    public static bool EsCampanaBorrador(string estado)
    {
        return Coincide(estado, "borrador");
    }

    public static bool EsCampanaProgramada(string estado)
    {
        return Coincide(estado, "programad");
    }

    public static bool EsCampanaEnviadaOFinalizada(string estado)
    {
        return Coincide(estado, "enviad", "finaliz");
    }

    private static bool Coincide(string? estado, params string[] fragmentos)
    {
        return !string.IsNullOrWhiteSpace(estado) &&
            fragmentos.Any(fragmento =>
                estado.Contains(fragmento, StringComparison.OrdinalIgnoreCase));
    }
}
