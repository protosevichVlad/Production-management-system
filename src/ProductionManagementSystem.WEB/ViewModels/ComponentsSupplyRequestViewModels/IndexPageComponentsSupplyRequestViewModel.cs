using System.Collections.Generic;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.WEB.ViewModels.ComponentsSupplyRequestViewModels
{
    public class IndexPageComponentsSupplyRequestViewModel
    {
        public IEnumerable<ComponentsSupplyRequest> ComponentsSupplyRequests { get; set; }
        public string OrderBy { get; set; }
        public string CurrentFilter { get; set; }

        public string Page { get; set; }
        public string PageSize { get; set; }
    }
}