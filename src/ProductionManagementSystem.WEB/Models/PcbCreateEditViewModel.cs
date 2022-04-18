using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ProductionManagementSystem.Core.Models.PCB;

namespace ProductionManagementSystem.WEB.Models
{
    public class PcbCreateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public DateTime ReportDate { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public List<EntityInPcb> Entities { get; set; }
        
        public IFormFile ImageUploader { get; set; }
        public IFormFile CircuitUploader { get; set; }
        public IFormFile AssemblyUploader { get; set; }
        public IFormFile TreeDUploader { get; set; }
    }
}