using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.PCB
{
    [Table("EntityInPCB")]
    public class EntityInPcb
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int Quantity { get; set; }
        public string Designator { get; set; }
        
        public Pcb Pcb { get; set; }
        public int PcbId { get; set; }
        
        [NotMapped]
        public EntityExt Entity { get; set; }
    }
}