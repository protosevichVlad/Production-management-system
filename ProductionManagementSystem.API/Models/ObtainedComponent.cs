namespace ProductionManagementSystem.API.Models
{
    public class ObtainedComponent
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public int TaskId { get; set; }
        public Component Component { get; set; }
        public int ComponentId { get; set; }
        public int Obtained { get; set; }
    }
}