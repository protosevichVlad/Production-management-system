using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.Tasks
{
    public abstract class ObtainedBase : BaseEntity
    {
        public int Obtained { get; set; }

        [NotMapped]
        public Task Task { get; set; }
        public int TaskId { get; set; }

        public int ComponentId { get; set; }
    }
}