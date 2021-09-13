namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDatabaseService
    {
        void ResetDatabase();

        void Dispose();
    }
}