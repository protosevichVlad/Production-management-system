using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using ProductionManagementSystem.Core.Exceptions;

namespace ProductionManagementSystem.WEB.Filters
{
    public class ApiException: Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled && context.Exception is DeleteReferenceException exception)
            {
                context.Result = new ContentResult()
                {
                    Content = JsonSerializer.Serialize(new
                    {
                        title = exception.Title,
                        message = exception.Message
                    }),
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
            else if (!context.ExceptionHandled)
            {
                var e = context.Exception;
                var data = JsonSerializer.Serialize(context.HttpContext.GetRouteData().Values as IDictionary<string, object>);
                string logPath = Path.Combine("wwwroot", "uploads", "log.txt");
                using var stream = new StreamWriter(logPath, true);
                stream.WriteLine($"{DateTime.Now} - {Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(context.HttpContext.Request)}\n" +
                                    $"Data: {data}\n" +
                                    $"Exception: {e.Message}\n" +
                                    $"InnerException: {e.InnerException?.Message ?? "-"}\n" +
                                    $"StackTrace: {e.ToString()}\n\n");
                
                context.ExceptionHandled = false;
            }
        }
    }
}