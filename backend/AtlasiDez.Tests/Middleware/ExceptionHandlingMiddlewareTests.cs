using System.Net;
using System.Text.Json;
using AtlasiDez.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AtlasiDez.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = Substitute.For<ILogger<ExceptionHandlingMiddleware>>();
    private readonly IHostEnvironment _environment = Substitute.For<IHostEnvironment>();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // [Fact]
    // public async Task WhenNoException_CompletesSuccessfully()
    // {
    //     var middleware = new ExceptionHandlingMiddleware(
    //         _ => Task.CompletedTask, _logger, _environment);
    //
    //     var context = new DefaultHttpContext();
    //
    //     await middleware.InvokeAsync(context);
    //
    //     Assert.Equal(200, context.Response.StatusCode);
    // }
    //
    // [Fact]
    // public async Task WhenArgumentException_Returns400BadRequest()
    // {
    //     RequestDelegate next = _ => throw new ArgumentException("parametro invalido");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (context, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
    //     Assert.Equal("Bad Request", problemDetails.Title);
    // }
    //
    // [Fact]
    // public async Task WhenHttpRequestException_Returns502BadGateway()
    // {
    //     RequestDelegate next = _ => throw new HttpRequestException("connection refused");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (context, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal((int)HttpStatusCode.BadGateway, context.Response.StatusCode);
    //     Assert.Equal("Bad Gateway", problemDetails.Title);
    // }
    //
    // [Fact]
    // public async Task WhenUnhandledException_Returns500InternalServerError()
    // {
    //     RequestDelegate next = _ => throw new InvalidOperationException("algo quebrou");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (context, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
    //     Assert.Equal("Internal Server Error", problemDetails.Title);
    // }
    //
    // [Fact]
    // public async Task WhenDevelopmentEnvironment_IncludesStackTrace()
    // {
    //     RequestDelegate next = _ => throw new InvalidOperationException("erro dev");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Development");
    //
    //     var (_, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal("erro dev", problemDetails.Detail);
    //     Assert.True(problemDetails.Extensions.ContainsKey("stackTrace"));
    // }
    //
    // [Fact]
    // public async Task WhenNonDevelopmentEnvironment_UsesGenericDetail()
    // {
    //     RequestDelegate next = _ => throw new InvalidOperationException("detalhe secreto");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (_, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal("Ocorreu um erro interno no servidor", problemDetails.Detail);
    //     Assert.False(problemDetails.Extensions.ContainsKey("stackTrace"));
    // }
    //
    // [Fact]
    // public async Task WhenNonDevelopmentEnvironment_BadGatewayUsesGenericDetail()
    // {
    //     RequestDelegate next = _ => throw new HttpRequestException("connection refused");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (_, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal("Erro ao comunicar com serviço externo", problemDetails.Detail);
    // }
    //
    // [Fact]
    // public async Task WhenArgumentExceptionInNonDev_DetailIsExceptionMessage()
    // {
    //     RequestDelegate next = _ => throw new ArgumentException("uf invalido");
    //     var middleware = new ExceptionHandlingMiddleware(next, _logger, _environment);
    //
    //     _environment.EnvironmentName.Returns("Production");
    //
    //     var (_, problemDetails) = await InvokeAndReadResponse(middleware);
    //
    //     Assert.Equal("uf invalido", problemDetails.Detail);
    // }
    //
    // private static async Task<(DefaultHttpContext context, ProblemDetails problemDetails)> InvokeAndReadResponse(
    //     ExceptionHandlingMiddleware middleware)
    // {
    //     var context = new DefaultHttpContext();
    //     context.Response.Body = new MemoryStream();
    //
    //     await middleware.InvokeAsync(context);
    //
    //     context.Response.Body.Seek(0, SeekOrigin.Begin);
    //     var problemDetails = await JsonSerializer.DeserializeAsync<ProblemDetails>(
    //         context.Response.Body, JsonOptions);
    //
    //     return (context, problemDetails!);
    // }
}
