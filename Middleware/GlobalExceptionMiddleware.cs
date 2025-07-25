using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Pampazon.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/problem+json";
            var (status, title) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, "No encontrado"),
                InvalidOperationException => (StatusCodes.Status400BadRequest, "Operación inválida"),
                ArgumentException => (StatusCodes.Status400BadRequest, "Argumento inválido"),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
            };
            context.Response.StatusCode = status;
            var problem = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = context.Request.Path
            };
            var json = JsonSerializer.Serialize(problem);
            await context.Response.WriteAsync(json);
        }
    }
}
