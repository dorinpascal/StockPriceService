using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stock_Price_Service.Models;
using System.Net;
using System.Security.Authentication;

namespace Stock_Price_Service.Helpers;

public static class HttpResponseMessageHelper
{
    public static ObjectResult HandleException(Exception exception, ILogger logger)
    {
        var (statusCode, message, errors) = exception switch
        {
            ArgumentNullException notFoundException =>
                (HttpStatusCode.NotFound, notFoundException.Message, null),
            FormatException formatException =>
                 (HttpStatusCode.BadRequest, formatException.Message, null),
            ArgumentException argException =>
                (HttpStatusCode.BadRequest, argException.Message, null),
            AuthenticationException authenticationException =>
                (HttpStatusCode.Unauthorized, authenticationException.Message, null),
            UnauthorizedAccessException unauthorizedException =>
                (HttpStatusCode.Forbidden, unauthorizedException.Message, null),
            _ =>
                (HttpStatusCode.ServiceUnavailable, "Service Unavailable", new List<string> { exception.Message })
        };

        logger.Log(statusCode == HttpStatusCode.ServiceUnavailable ? LogLevel.Error : LogLevel.Information, message);
        var responseBody = new ErrorResponseBody
        {
            Message = message,
            Errors = errors ?? []
        };

        return new ObjectResult(responseBody) { StatusCode = (int)statusCode };
    }
}
