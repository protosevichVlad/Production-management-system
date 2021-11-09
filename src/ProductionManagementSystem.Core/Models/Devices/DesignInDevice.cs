using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Devices
{
    [Table("DeviceDesignTemplates")]
    public class DesignInDevice : ComponentBaseInDevice<Design>
    {
        
    }
}