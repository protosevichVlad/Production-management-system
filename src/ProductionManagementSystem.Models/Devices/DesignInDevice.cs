using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.Devices
{
    [Table("DeviceDesignTemplates")]
    public class DesignInDevice : ComponentBaseInDevice<Design>
    {
        
    }
}