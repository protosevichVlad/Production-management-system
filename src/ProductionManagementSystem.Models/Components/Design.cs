namespace ProductionManagementSystem.Models.Components
{
    public class Design : ComponentBase
    {
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {ShortDescription}";
        }
    }
}
