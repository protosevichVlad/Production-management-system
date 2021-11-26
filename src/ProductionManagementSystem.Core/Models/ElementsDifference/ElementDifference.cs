using System;

namespace ProductionManagementSystem.Core.Models.ElementsDifference
{
    public class ElementDifference
    {
        public ElementType ElementType { get; set; }
        public int ElementId { get; set; }
        public int Difference { get; set; }
        public DateTime DateTime { get; set; }
    }
}