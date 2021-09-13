using System;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Enums;

namespace ProductionManagementSystem.BLL.DTO
{
    public class DesignsSupplyRequestDTO
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public OrderDTO Order { get; set; }
        public int DesignId { get; set; }
        public DesignDTO Design { get; set; }
        public UserDTO User { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DesiredDate { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public StatusSupplyEnumDTO StatusSupply { get; set; }
    }
}