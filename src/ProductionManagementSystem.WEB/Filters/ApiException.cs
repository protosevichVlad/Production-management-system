using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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
        }
    }
}