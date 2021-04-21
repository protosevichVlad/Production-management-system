namespace ProductionManagementSystem.BLL.DTO
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public int[] DesignIds { get; set; }
        public int[] ComponentIds { get; set; }
        public int[] DesignQuantity { get; set; }
        public int[] ComponentQuantity { get; set; }
        public string[] DesignDescriptions { get; set; }
        public string[] ComponentDescriptions { get; set; }
        public string[] DesignNames { get; set; }
        public string[] ComponentNames { get; set; }
        public int[] DesignTemplateId { get; set; }
        public int[] ComponentTemplateId { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Description))
            {
                return Name;
            }
            
            string result = Name + " ";
            if (Description.Length > 25)
            {
                result += Description[..25] + "..";
            }
            else
            {
                result += Description;
            }

            return result;
        }
    }
}