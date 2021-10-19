namespace ProductionManagementSystem.Models.Components
{
    public class Montage : ComponentBase
    {
        public string Nominal { get; set; }
        public string Corpus { get; set; }
        public string Explanation { get; set; }
        public string Manufacturer { get; set; }
        
        public override string ToString()
        {
            return $"{Type} {Name} {Nominal}";
        }
    }
}
