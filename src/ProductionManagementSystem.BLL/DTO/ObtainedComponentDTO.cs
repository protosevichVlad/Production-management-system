namespace ProductionManagementSystem.BLL.DTO
{
    public class ObtainedComponentDTO
    {
        public int Id { get; set; }
        public TaskDTO Task { get; set; }
        public int TaskId { get; set; }
        public ComponentDTO Component { get; set; }
        public int ComponentId { get; set; }
        public int Obtained { get; set; }
    }
}