namespace ProductionManagementSystem.DAL.Entities
{
    public class Component
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
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
