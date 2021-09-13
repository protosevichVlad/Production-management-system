using System;
using ProductionManagementSystem.DAL.Enums;

namespace ProductionManagementSystem.DAL.Entities
{
    public class ComponentsSupplyRequest
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public Task Task { get; set; }
        public int ComponentId { get; set; }
        public Component Component { get; set; }
        public ProductionManagementSystemUser User { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DesiredDate { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public StatusSupplyEnum StatusSupply { get; set; }
    }
}