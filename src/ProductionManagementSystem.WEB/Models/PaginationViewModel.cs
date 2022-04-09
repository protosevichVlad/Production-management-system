namespace ProductionManagementSystem.WEB.Models
{
    public class PaginationViewModel
    {
        public int ItemPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalItems { get; set; }
    }
}