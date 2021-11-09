using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProductionManagementSystem.WEB.Models.Modals
{
    public class ModalSupplyRequest : ModalBase
    {
        public int SupplyRequestId { get; set; }
        public SelectList States { get; set; }
    }
}