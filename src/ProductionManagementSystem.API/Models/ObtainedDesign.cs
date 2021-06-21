namespace ProductionManagementSystem.API.Models
{
    public class ObtainedDesign
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int DesignId { get; set; }
        public int Obtained { get; set; }
    }
}