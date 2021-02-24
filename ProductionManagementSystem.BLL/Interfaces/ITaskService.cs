using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ITaskService
    {
        void CreateTask(TaskDTO deviceDto);
        void UpdateTask(TaskDTO deviceDto);
        IEnumerable<TaskDTO> GetTasks();
        TaskDTO GetTask(int? id);
        void DeleteTask(int? id);
        
        void Dispose();
    }
}