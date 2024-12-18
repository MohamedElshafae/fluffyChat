
using HR_System.Application.Helpers;
using QR_Domain.Exceptions;


namespace HR.API.Middlewares
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly LogManager _logManger;
        public GlobalExceptionHandler(RequestDelegate next, LogManager logManger)
        {
            _logManger = logManger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                await _logManger.WriteLogs(ex.Message, ex.Id);
                await PrepareHttpResponse(context.Response, ex);
            }
            catch (Exception ex)
            {
                var id = Guid.NewGuid();

                await _logManger.WriteLogs(ex.Message, id);
                await PrepareHttpResponse(context.Response, ex, id);
            }
        }

        private async Task PrepareHttpResponse(HttpResponse httpResponse, BusinessException exception)
        {
            httpResponse.StatusCode = exception.StatusCode;
            httpResponse.ContentType = "application/json";
            var result = new {MessageId = exception.Id, StatusCode = exception.StatusCode, Success = false, exception.Message};

            await httpResponse.WriteAsJsonAsync(result);
        }

        private async Task PrepareHttpResponse(HttpResponse httpResponse, Exception exception, Guid messageId)
        {
            httpResponse.ContentType = "application/json";
            var result = new { MessageId = messageId, StatusCode = (int)HttpStatusCode.InternalServerError, Success = false, exception.Message };

            await httpResponse.WriteAsJsonAsync(result);
        }
    }
}
