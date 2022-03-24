using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Models.Infrastructure
{
    public static class Extensions
    {
        public static string GetTasksStatusName(this TaskStatusEnum enumValue) 
        {
            List<string> result = new List<string>();
            foreach (var value in Enum.GetValues<TaskStatusEnum>())
                if ((enumValue & value) == value)
                {
                    result.Add(value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName());
                }
            
            return String.Join(", ", result);
        }
    }
}