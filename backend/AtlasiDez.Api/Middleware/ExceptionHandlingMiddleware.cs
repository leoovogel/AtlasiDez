using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace AtlasiDez.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException ex => (HttpStatusCode.BadRequest, "Bad Request", ex.Message),
            HttpRequestException => (HttpStatusCode.BadGateway, "Bad Gateway", "Erro ao comunicar com serviço externo"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error", "Ocorreu um erro interno no servidor")
        };

        LogException(exception, statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = environment.IsDevelopment() ? exception.Message : detail
        };

        if (environment.IsDevelopment())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private void LogException(Exception exception, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Exceção não tratada: {Message}", exception.Message);
        else if (statusCode == HttpStatusCode.BadGateway)
            logger.LogWarning(exception, "Falha em serviço externo: {Message}", exception.Message);
        else
            logger.LogInformation(exception, "Exceção tratada ({StatusCode}): {Message}", (int)statusCode, exception.Message);
    }
}
