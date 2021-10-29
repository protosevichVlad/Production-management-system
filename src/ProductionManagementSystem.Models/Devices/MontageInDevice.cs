using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Models.Components;

namespace ProductionManagementSystem.Models.Devices
{
    [Table("DeviceComponentsTemplates")]
    public class MontageInDevice : ComponentBaseInDevice<Montage>
    {
        
    }
}