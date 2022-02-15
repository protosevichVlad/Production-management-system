using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.Devices
{
    [Table("Devices")]
    public class Device : CountingEntity
    {
        [Display(Name = "Описание")]
        public string Description { get; set; }
        
        [NotMapped]
        public List<DesignInDevice> Designs { get; set; }
        [NotMapped]
        public List<MontageInDevice> Montages { get; set; }
        
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Description))
            {
                return Name;
            }
            
            string result = Name + " ";
            if (Description.Length > 25)
            {
                result += Description[..25] + "..";
            }
            else
            {
                result += Description;
            }

            return result;
        }
    }
}