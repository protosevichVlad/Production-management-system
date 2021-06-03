namespace ProductionManagementSystem.API.Models
{
public class DeviceDesignTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int DesignId { get; set; }
        public string Description { get; set; }
    }
}
