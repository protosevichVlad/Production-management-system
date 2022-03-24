using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.Logs;
using ProductionManagementSystem.Core.Models.SupplyRequests;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.WEB.Models.Tasks
{
    public class TaskDetailsViewModel
    {
        public Task Task { get; set; }
        public List<SupplyRequest> SupplyRequests { get; set; }
        public List<Log> Logs { get; set; }
    }
}