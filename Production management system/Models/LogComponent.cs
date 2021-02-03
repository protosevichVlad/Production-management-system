namespace ProductionManagementSystem.Models
{
    public class LogComponent
    {
        public int Id { get; set; }
        public Component Component { get; set; }
        public int? ComponentId { get; set; }
        public Log Log { get; set; }
        public int? LogId { get; set; }
        
    }
}