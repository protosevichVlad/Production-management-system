﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.Users;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Controllers;

namespace ProductionManagementSystem.WEB.Filters
{
    public class ToDoActionFilter : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Controller is AltiumDBController controller)
            {
                var service = context.HttpContext.RequestServices.GetService<IToDoNoteService>();
                if (service == null)
                    controller.ViewBag.ToDos = new List<ToDoNote>();
                else
                    controller.ViewBag.ToDos = (await service.GetAllAsync()).OrderByDescending(x => x.Created).ToList();
            }
            
            await base.OnResultExecutionAsync(context, next);
        }
    }
}