using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace API.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleCustomExceptionResponseAsync(context, ex);
            }
        }

        private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new ErrorModel(context.Response.StatusCode, ex.Message);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
    public class ErrorModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ErrorModel(int statusCode, string? message)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}

