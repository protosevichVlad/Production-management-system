using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Areas.AltiumDB.Controllers;
using ProductionManagementSystem.WEB.Controllers;

namespace ProductionManagementSystem.WEB.Filters
{
    public class UserFilter : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Controller is Controller controller)
            {
                var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
                var userService = context.HttpContext.RequestServices.GetService<IUserService>();
                if (userManager != null && userService != null && controller.User.Identity != null && !string.IsNullOrEmpty(controller.User.Identity.Name))
                        userService.User = await userManager.FindByNameAsync(controller.User.Identity.Name);
            }

            await base.OnResultExecutionAsync(context, next);
        }
    }
}