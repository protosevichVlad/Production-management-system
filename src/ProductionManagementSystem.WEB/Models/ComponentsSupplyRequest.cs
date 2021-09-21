using System;

namespace ProductionManagementSystem.WEB.Models
{
    public class ComponentsSupplyRequest
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public TaskViewModel Task { get; set; }
        public int ComponentId { get; set; }
        public ComponentViewModel Component { get; set; }
        public UserViewModel User { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DesiredDate { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public StatusSupplyEnum StatusSupply { get; set; }
    }
}