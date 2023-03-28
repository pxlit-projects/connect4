using ConnectFour.Api.Models;
using ConnectFour.Common;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ConnectFour.Api.Filters;

public class ConnectFourExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly ILogger _logger;

    public ConnectFourExceptionFilterAttribute(ILogger logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is DataNotFoundException)
        {
            _logger.LogWarning(context.Exception,
                $"Client asked for a resource that does not exist. Request: {GetRequestUrl(context)}");

            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Result = new NotFoundResult();
        }
        else if (context.Exception is ApplicationException || context.Exception is InvalidOperationException)
        {
            _logger.LogWarning(context.Exception,
                $"Invalid client input caused an exception. Request: {GetRequestUrl(context)}");

            context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Result = new JsonResult(new ErrorModel(context.Exception));
        }
        else
        {
            _logger.LogError(context.Exception,
                $"An unhandled exception occurred in the application. Request: {GetRequestUrl(context)}");

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new JsonResult(new ErrorModel(context.Exception));
        }


    }

    private string GetRequestUrl(ExceptionContext context)
    {
        if (context.HttpContext?.Request == null) return string.Empty;
        return $"{context.HttpContext.Request.Method} - {context.HttpContext.Request.GetDisplayUrl()}";
    }
}