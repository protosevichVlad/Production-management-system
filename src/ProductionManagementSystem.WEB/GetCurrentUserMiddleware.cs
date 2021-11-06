using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ProductionManagementSystem.BLL.Services;

namespace ProductionManagementSystem.WEB
{
    public class GetCurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public GetCurrentUserMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
 
        public async Task InvokeAsync(HttpContext context, ILogService logService)
        {
            if (context.User.Identity?.Name != null)
                logService.CurrentUserName = context.User.Identity?.Name;
            await _next.Invoke(context);
        }
    }
}