using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Models.Devices
{
    [Table("Devices")]
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
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