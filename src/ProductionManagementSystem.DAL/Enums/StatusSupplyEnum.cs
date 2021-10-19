using System;
using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.DAL.Enums
{
    [Flags]
    public enum StatusSupplyEnum
    {
        [Display(Name="Не принята")]
        NotAccepted = 1,
        [Display(Name="В работе")]
        InWork = 2,
        [Display(Name="Заказано")]
        Ordered = 4,
        [Display(Name="Готова")]
        Ready = 8,
    }
}