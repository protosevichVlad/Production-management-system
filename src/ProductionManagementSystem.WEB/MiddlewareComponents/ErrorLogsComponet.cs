using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace ProductionManagementSystem.WEB.MiddlewareComponents
{
    public class ErrorLogsComponent
    {
        private readonly RequestDelegate _next;
 
        public ErrorLogsComponent(RequestDelegate next)
        {
            this._next = next;
        }
 
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await this._next.Invoke(context);
            }
            catch (Exception e)
            {
                var controllerName = context.GetRouteData().Values["controller"];
                var actionName = context.GetRouteData().Values["action"];
                var data = JsonSerializer.Serialize(context.GetRouteData().Values as IDictionary<string, object>);
                string logPath = Path.Combine(".", "log.txt");
                await using var stream = new StreamWriter(logPath, append: true);
                await stream.WriteLineAsync($"{DateTime.Now} - {controllerName}:{actionName}\n" +
                                            $"Data: {data}\n" +
                                            $"Exception: {e.Message}\n" +
                                            $"InnerException: {e.InnerException?.Message ?? "-"}\n" +
                                            $"StackTrace: {e.ToString()}\n");
                throw;
            }
        }
    }
}