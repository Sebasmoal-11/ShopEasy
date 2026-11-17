namespace ShopEasy.API.Errors;

/// <summary>
/// Decide cuando una respuesta de error debe mostrarse como pagina HTML y
/// ubica los archivos de error del FrontEnd. Las rutas /api siempre conservan
/// sus respuestas JSON.
/// </summary>
public static class ErrorPages
{
    private static readonly Dictionary<int, string> Archivos = new()
    {
        [StatusCodes.Status404NotFound] = "404.html",
        [StatusCodes.Status500InternalServerError] = "500.html",
        [StatusCodes.Status502BadGateway] = "502.html",
        [StatusCodes.Status503ServiceUnavailable] = "503.html",
        [StatusCodes.Status504GatewayTimeout] = "504.html"
    };

    public static bool EsSolicitudApi(HttpContext context)
    {
        return context.Request.Path.StartsWithSegments("/api");
    }

    public static bool AceptaHtml(HttpContext context)
    {
        return context.Request.Headers.Accept
            .Any(valor => valor?.Contains("text/html", StringComparison.OrdinalIgnoreCase) == true);
    }

    public static bool DebeMostrarPagina(HttpContext context)
    {
        return !EsSolicitudApi(context) && AceptaHtml(context);
    }

    public static string? ArchivoPara(int statusCode)
    {
        return Archivos.TryGetValue(statusCode, out var archivo) ? archivo : null;
    }

    public static string Carpeta(string contentRootPath)
    {
        return Path.GetFullPath(
            Path.Combine(contentRootPath, "..", "..", "FrontEnd", "Errores"));
    }
}
