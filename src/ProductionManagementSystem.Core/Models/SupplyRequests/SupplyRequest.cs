using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Models.SupplyRequests
{
    public abstract class SupplyRequest : BaseEntity
    {
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        public string Comment { get; set; }
        
        [Display(Name = "Дата добавления")]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }
        
        [Display(Name = "Желаемая дата")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime DesiredDate { get; set; }
        
        [Display(Name = "Статус")]
        public SupplyStatusEnum StatusSupply { get; set; }

        [NotMapped]
        public string StatusRuName
        {
            get 
            {
                string result = "";
                foreach (var value in Enum.GetValues<SupplyStatusEnum>())
                {
                    if (this.StatusSupply == value)
                    {
                        result = value.GetType()
                            .GetMember(value.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            ?.GetName();
                        
                        break;;
                    }
                }

                return result;
            }
        }
        
        public Task Task { get; set; }
        
        [Display(Name = "Задача №")]
        public int? TaskId { get; set; }
        
        public Users.User User { get; set; }
        [Display(Name = "Пользователь")]
        public string UserId { get; set; }
        
        public int ComponentId { get; set; }

        public override string ToString()
        {
            return $"№{this.Id}";
        }
    }
}