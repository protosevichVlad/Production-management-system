namespace ProductionManagementSystem.Models
{
    public class LogDesign
    {
        public int Id { get; set; }
        public Design Design { get; set; }
        public int? DesignId { get; set; }
        public Log Log { get; set; }
        public int? LogId { get; set; }
    }
}