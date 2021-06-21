namespace ProductionManagementSystem.API.Models
{
    public class Design
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {ShortDescription}";
        }
    }
}
