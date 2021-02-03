namespace ProductionManagementSystem.ViewModels
{
    public class DeviceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int[] DesignIds { get; set; }
        public int[] ComponentIds { get; set; }
        public int[] DesignQuantity { get; set; }
        public int[] ComponentQuantity { get; set; }
        public string[] DesignDescriptions { get; set; }
        public string[] ComponentDescriptions { get; set; }
    }
}