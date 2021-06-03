namespace ProductionManagementSystem.BLL.DTO
{
    public class ObtainedDesignDTO
    {
        public int Id { get; set; }
        public TaskDTO Task { get; set; }
        public int TaskId { get; set; }
        public DesignDTO Design { get; set; }
        public int DesignId { get; set; }
        public int Obtained { get; set; }
    }
}