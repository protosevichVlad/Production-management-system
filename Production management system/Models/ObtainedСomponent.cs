namespace ProductionManagementSystem.Models
{
    public class ObtainedСomponent
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public Component Component { get; set; }
        public int Obtained { get; set; }
    }
}