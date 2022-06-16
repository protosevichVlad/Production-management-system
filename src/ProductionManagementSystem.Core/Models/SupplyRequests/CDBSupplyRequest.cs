using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using ProductionManagementSystem.Core.Models.SupplyRequests;

namespace ProductionManagementSystem.Core.Models
{
    public class CDBSupplyRequest
    {
        public int Id { get; set; }
        
        [Display(Name = "Количество")]
        public int Quantity { get; set; }
        
        [Display(Name = "Описание")]
        public string Comment { get; set; }
        
        [Display(Name = "Дата добавления")]
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
        
        public CDBTask Task { get; set; }
        
        [Display(Name = "Задача №")]
        public int? TaskId { get; set; }
        
        public Users.User User { get; set; }
        [Display(Name = "Пользователь")]
        public string UserId { get; set; }
        
        public int ItemId { get; set; }
        [NotMapped]
        public EntityExt Entity { get; set; }

        public override string ToString()
        {
            return $"№{this.Id}" + (Entity != null ? $"{Entity} {Quantity}шт.":"");
        }
    }
}