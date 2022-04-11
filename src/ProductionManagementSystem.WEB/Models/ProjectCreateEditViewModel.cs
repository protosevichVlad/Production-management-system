using System;

namespace ProductionManagementSystem.WEB.Models
{
    public class ProjectCreateEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Variant { get; set; }
        public DateTime ReportDate { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
}