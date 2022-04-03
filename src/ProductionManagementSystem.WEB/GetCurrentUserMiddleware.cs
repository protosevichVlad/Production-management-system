using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;

namespace ProductionManagementSystem.WEB
{
    public class GetCurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public GetCurrentUserMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
 
        public async Task InvokeAsync(HttpContext context, ILogService logService, UserManager<User> userManager, IUserService userService)
        {
            if (context.User.Identity?.Name != null)
            {
                logService.CurrentUserName = context.User.Identity?.Name;
                userService.User = await userManager.GetUserAsync(context.User);
            }
            
            await _next.Invoke(context);
        }
    }
}