﻿using System.Collections.Generic;

namespace ProductionManagementSystem.API.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<DeviceComponentsTemplate> DeviceComponentsTemplate { get; set; }
        public List<DeviceDesignTemplate> DeviceDesignTemplate { get; set; }
        
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
