using System;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Enums;

namespace ProductionManagementSystem.BLL.DTO
{
    public class ComponentsSupplyRequestDTO
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public Task Task { get; set; }
        public int ComponentId { get; set; }
        public ComponentDTO Component { get; set; }
        public UserDTO User { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DesiredDate { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public StatusSupplyEnumDTO StatusSupply { get; set; }
    }
}