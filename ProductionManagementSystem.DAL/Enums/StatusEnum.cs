using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.Models
{
    [Flags]
    public enum StatusEnum
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
}