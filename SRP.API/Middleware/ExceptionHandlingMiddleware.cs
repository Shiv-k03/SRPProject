using SRP.Model.Helper.Base;
using SRP.Repository.Exceptions;
using System.Net;
using System.Text.Json;

namespace SRP.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var errorResponse = new ErrorResponse
            {
                Message = "An internal error occurred",
                StatusCode = (int)statusCode,
                Timestamp = DateTime.UtcNow,
                TraceId = context.TraceIdentifier,
                Path = context.Request.Path
            };

            switch (exception)
            {
                case NotFoundException notFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    errorResponse.Message = notFoundException.Message;
                    errorResponse.StatusCode = (int)statusCode;
                    break;

                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = badRequestException.Message;
                    errorResponse.StatusCode = (int)statusCode;
                    break;

                case UnauthorizedException unauthorizedException:
                    statusCode = HttpStatusCode.Unauthorized;
                    errorResponse.Message = unauthorizedException.Message;
                    errorResponse.StatusCode = (int)statusCode;
                    break;

                case ValidationException validationException:
                    statusCode = HttpStatusCode.BadRequest;
                    errorResponse.Message = validationException.Message;
                    errorResponse.StatusCode = (int)statusCode;
                    errorResponse.Errors = validationException.Errors;
                    break;

                default:
                    errorResponse.Message = "An unexpected error occurred";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(result);
        }
    }
}
