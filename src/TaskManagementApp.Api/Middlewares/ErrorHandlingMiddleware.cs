using System.Net;
using System.Text.Json;
using TaskManagementApp.Models.Errors;

namespace TaskManagementApp.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu uma exceção não tratada: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorResponse = new ErrorResponse("SERVER_ERROR", "Ocorreu um erro interno do servidor.");

            if (exception is ArgumentException argEx)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse("INVALID_ARGUMENT", argEx.Message);
            }

            var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}