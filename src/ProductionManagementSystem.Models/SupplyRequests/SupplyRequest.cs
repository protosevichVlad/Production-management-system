using System;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;
using ProductionManagementSystem.Models.Tasks;

namespace ProductionManagementSystem.Models.SupplyRequests
{
    public abstract class SupplyRequest<TComponent> where TComponent : ComponentBase 
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DesiredDate { get; set; }
        public SupplyStatusEnum StatusSupply { get; set; }
        
        
        [NotMapped]
        public Task Task { get; set; }
        public int? TaskId { get; set; }
        
        [NotMapped]
        public TComponent Component { get; set; }
        public int ComponentId { get; set; }
        
        [NotMapped]
        public Users.User User { get; set; }
        public string UserId { get; set; }
    }
}