using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Components;

namespace ProductionManagementSystem.Core.Models.Devices
{
    [Table("DeviceComponentsTemplates")]
    public class MontageInDevice : ComponentBaseInDevice<Montage>
    {
        
    }
}