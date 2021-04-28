using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Money.Api.Dto;
using Money.Api.Enums;
using Money.Api.Exceptions;
using Money.Api.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Money.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e, logger);
            }
        }

        private Task HandleExceptionAsync(
            HttpContext context,
            Exception exception,
            ILogger logger)
        {
            logger.LogInformation($"{nameof(HandleExceptionAsync)}: Handling exception of type = {exception.GetType()}....");

            var code = HttpStatusCode.InternalServerError;

            var response = new EmptyResponseDto
            {
                ErrorMessage = AppErrorType.AppUnknownErrorOccurred.GetErrorMsg(),
                ErrorMessageId = AppErrorType.AppUnknownErrorOccurred.GetErrorCode(),
            };
            context.Response.ContentType = "application/json";
            switch (exception)
            {
                case ValidationException validationEx:
                    code = HttpStatusCode.BadRequest;
                    response.ErrorMessageId = validationEx.ErrorMessageId;
                    response.ErrorMessage = validationEx.Error;
                    break;
                case CurrencyException currencyEx:
                    code = HttpStatusCode.InternalServerError;
                    response.ErrorMessageId = currencyEx.ErrorType.GetErrorCode();
                    response.ErrorMessage = currencyEx.ErrorType.GetErrorMsg();
                    break;
                case NotFoundException notFoundEx:
                    code = HttpStatusCode.NotFound;
                    response.ErrorMessageId = notFoundEx.ErrorType.GetErrorCode();
                    response.ErrorMessage = notFoundEx.ErrorType.GetErrorMsg();
                    break;
                case InvalidRequestException invEx:
                    code = HttpStatusCode.BadRequest;
                    response.ErrorMessageId = invEx.ErrorType.GetErrorCode();
                    response.ErrorMessage = invEx.ErrorType.GetErrorMsg();
                    break;
                default:
                    logger.LogError(exception, $"{nameof(HandleExceptionAsync)}: Unknown exception was captured");
                    break;
            }
#if DEBUG
            response.ErrorMessage += $". Ex: {exception}";
#endif
            context.Response.StatusCode = (int)code;

            logger.LogInformation(
                $"{nameof(HandleExceptionAsync)}: The final response is going to " +
                $"be = {response.ErrorMessageId} - {response.ErrorMessage}");

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response, SerializerSettings));
        }
    }
}
