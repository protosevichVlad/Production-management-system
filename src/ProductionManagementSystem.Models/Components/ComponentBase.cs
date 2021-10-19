namespace ProductionManagementSystem.Models.Components
{
    public class ComponentBase
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name}";
        }
    }
}
