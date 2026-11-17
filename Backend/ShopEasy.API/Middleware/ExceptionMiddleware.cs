using Microsoft.EntityFrameworkCore;
using ShopEasy.API.Errors;
using ShopEasy.BC.Common;

namespace ShopEasy.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
        }
        catch (ApiException ex)
        {
            await EscribirErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (BadHttpRequestException)
        {
            await EscribirErrorAsync(
                context,
                StatusCodes.Status400BadRequest,
                "El cuerpo de la solicitud no tiene un formato valido.");
        }
        catch (DbUpdateException)
        {
            await EscribirErrorAsync(
                context,
                StatusCodes.Status409Conflict,
                "No se pudo guardar porque los datos entran en conflicto con un registro existente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Metodo} {Ruta}.", context.Request.Method, context.Request.Path);
            await EscribirErrorAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocurrio un error inesperado. Intenta de nuevo.");
        }
    }

    private async Task EscribirErrorAsync(HttpContext context, int statusCode, string mensaje)
    {
        context.Response.StatusCode = statusCode;

        if (ErrorPages.DebeMostrarPagina(context))
        {
            var archivo = ErrorPages.ArchivoPara(statusCode);
            var ruta = archivo is null
                ? null
                : Path.Combine(ErrorPages.Carpeta(_environment.ContentRootPath), archivo);

            if (ruta is not null && File.Exists(ruta))
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync(ruta);
                return;
            }
        }

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            message = mensaje
        });
    }
}
