using System.ComponentModel.DataAnnotations;

namespace ProductionManagementSystem.WEB.Models
{
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