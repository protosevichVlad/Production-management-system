using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.ElementsDifference
{
    public class ElementDifference
    {
        public int Id { get; set; }
        public ElementType ElementType { get; set; }
        [NotMapped]
        public CountingEntity Element { get; set; }
        public int ElementId { get; set; }
        public int Difference { get; set; }
        public DateTime DateTime { get; set; }

        public ElementDifference()
        {
            this.DateTime = DateTime.Now;
        }
    }
}