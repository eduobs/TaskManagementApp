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
            var statusCode = HttpStatusCode.InternalServerError;
            var errorResponse = new ErrorResponse("SERVER_ERROR", "Ocorreu um erro interno do servidor.");

            switch (exception)
            {
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse = new ErrorResponse("INVALID_ARGUMENT", argEx.Message);
                    break;
                case InvalidOperationException invOpEx:
                    statusCode = HttpStatusCode.Conflict;
                    errorResponse = new ErrorResponse("BUSINESS_RULE_VIOLATION", invOpEx.Message);
                    break;
                default:
                    _logger.LogError(exception, "Exceção não mapeada capturada pelo middleware: {ExceptionType}", exception.GetType().Name);
                    break;
            }

            context.Response.StatusCode = (int)statusCode;
            var jsonResponse = JsonSerializer.Serialize(errorResponse, _jsonSerializerOptions);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
