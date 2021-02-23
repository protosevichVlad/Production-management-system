namespace ProductionManagementSystem.BLL.DTO
{
    public class ComponentDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Nominal { get; set; }
        public string Corpus { get; set; }
        public string Explanation { get; set; }
        public string Manufacturer { get; set; }
        
        // TODO delete and add to viewModel
        public override string ToString()
        {
            return $"{Type} {Name} {Nominal}";
        }
    }
}