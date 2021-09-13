namespace ProductionManagementSystem.API.Models
{
    public class DeviceComponentsTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ComponentId { get; set; }
        public string Description { get; set; }
    }
}
