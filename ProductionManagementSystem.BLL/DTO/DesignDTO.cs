namespace ProductionManagementSystem.BLL.DTO
{
    public class DesignDTO
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        
        // TODO delete and add to viewModel
        public override string ToString()
        {
            return $"{Type} {Name} {ShortDescription}";
        }
    }
}