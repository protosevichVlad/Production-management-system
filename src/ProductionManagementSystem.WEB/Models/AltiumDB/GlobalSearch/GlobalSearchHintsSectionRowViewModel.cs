using ProductionManagementSystem.Core.Models;

namespace ProductionManagementSystem.WEB.Models.AltiumDB.GlobalSearch
{
    public enum HintsSectionRowType
    {
        Text,
        BaseEntity,
        Pcb,
        Table
    }
    
    public class GlobalSearchHintsSectionRowViewModel
    {
        public HintsSectionRowType Type { get; set; }
        public object Content { get; set; }
    }
}