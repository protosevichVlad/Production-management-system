namespace ProductionManagementSystem.DAL.Entities
{
    public class DeviceComponentsTemplate
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Component Component { get; set; }
        public int ComponentId { get; set; }
        public int DeviceId { get; set; }
        public string Description { get; set; }


    }
}
