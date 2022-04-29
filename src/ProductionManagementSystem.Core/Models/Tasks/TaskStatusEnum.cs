using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ProductionManagementSystem.Core.Models.Tasks
{
    [Flags]
    public enum TaskStatusEnum
    {
        [Display(Name="Комплектация")]
        Equipment = 1,
        [Display(Name="Монтаж")]
        Montage = 2,
        [Display(Name="Настройка")]
        Customization = 4,
        [Display(Name="Сборка")]
        Assembly = 8,
        [Display(Name="Проверка")]
        Validate = 16,
        [Display(Name="Склад")]
        Warehouse = 32,
        [Display(Name="Задача выполнена")]
        Done = 64
    }

    public static class TaskStatusExtension
    {
        public static string GetName(this TaskStatusEnum status)
        {
            List<string> result = new List<string>();
            foreach (var value in Enum.GetValues<TaskStatusEnum>())
            {
                if ((status & value) == value)
                {
                    result.Add(value.GetType()
                        .GetMember(value.ToString())
                        .First()
                        .GetCustomAttribute<DisplayAttribute>()
                        ?.GetName());
                }
            }
            return String.Join(", ", result);
        }
    }
}