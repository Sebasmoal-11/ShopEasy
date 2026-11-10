using Microsoft.EntityFrameworkCore;
using ShopEasy.BC.Common;

namespace ShopEasy.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

    private static async Task EscribirErrorAsync(HttpContext context, int statusCode, string mensaje)
    {
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            message = mensaje
        });
    }
}
