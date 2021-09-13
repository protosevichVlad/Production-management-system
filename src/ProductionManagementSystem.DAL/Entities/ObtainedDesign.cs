namespace ProductionManagementSystem.DAL.Entities
{
    public class ObtainedDesign
    {
        public int Id { get; set; }
        public Task Task { get; set; }
        public int TaskId { get; set; }
        public Design Design { get; set; }
        public int DesignId { get; set; }
        public int Obtained { get; set; }
    }
}