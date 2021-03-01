namespace ProductionManagementSystem.DAL.Entities
{
public class DeviceDesignTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Design Design { get; set; }
        public int DesignId { get; set; }
        public int DeviceId { get; set; }
        public string Description { get; set; }

    }
}
