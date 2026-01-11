using System.Text.Json;

namespace TrafficIncidentsOpitech.Api.Middleware;

public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex,
                "Error de validacion (ArgumentOutOfRangeException). Path: {Path}. Param: {Param}. Message: {Message}",
                context.Request.Path, ex.ParamName, ex.Message);

            await WriteJsonAsync(context, StatusCodes.Status400BadRequest, new
            {
                error = "ValidationError",
                message = ex.Message,
                param = ex.ParamName
            });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex,
                "Error de validacion (ArgumentException). Path: {Path}. Param: {Param}. Message: {Message}",
                context.Request.Path, ex.ParamName, ex.Message);

            await WriteJsonAsync(context, StatusCodes.Status400BadRequest, new
            {
                error = "ValidationError",
                message = ex.Message,
                param = ex.ParamName
            });
        }
        catch (FluentValidation.ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validacion. Ruta: {Path}", context.Request.Path);

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            await WriteJsonAsync(context, StatusCodes.Status400BadRequest, new
            {
                error = "ValidationError",
                message = "El request es inválido.",
                errors,
                traceId = context.TraceIdentifier
            });
        }

        catch (Exception ex)
        {            
            _logger.LogError(ex, "Excepción no controlada. Path: {Path}. Method: {Method}",
                context.Request.Path, context.Request.Method);
            
            if (_env.IsDevelopment())
            {
                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError, new
                {
                    error = "ServerError",
                    message = ex.Message,
                    inner = ex.InnerException?.Message,
                    traceId = context.TraceIdentifier
                });
                return;
            }

            await WriteJsonAsync(context, StatusCodes.Status500InternalServerError, new
            {
                error = "ServerError",
                message = "Ocurrió un error inesperado."
            });
        }
    }

    private static Task WriteJsonAsync(HttpContext context, int statusCode, object payload)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
