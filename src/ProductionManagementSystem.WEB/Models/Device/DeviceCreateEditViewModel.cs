using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.WEB.Models.Device
{
    public class DeviceCreateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public DateTime ReportDate { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<UsedItem> UsedItems { get; set; }
        
        public IFormFile ImageUploader { get; set; }
        public IFormFile ThreeDModelUploader { get; set; }
    }
}