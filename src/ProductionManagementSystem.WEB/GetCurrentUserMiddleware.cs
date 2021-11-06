using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProductionManagementSystem.BLL.Services;
using ProductionManagementSystem.Models.Users;

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
            logService.CurrentUserName = context.User.Identity?.Name;
            await _next.Invoke(context);
        }
    }
}