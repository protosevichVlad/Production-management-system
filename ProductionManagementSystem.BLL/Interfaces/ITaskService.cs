using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.Models;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ITaskService
    {
        Task CreateTaskAsync(TaskDTO taskDto);
        Task UpdateTaskAsync(TaskDTO taskDto);
        Task EditTaskAsync(TaskDTO taskDto);
        Task<IEnumerable<TaskDTO>> GetTasksAsync();
        Task<IEnumerable<TaskDTO>> GetTasksAsync(IEnumerable<string> roles);
        Task<TaskDTO> GetTaskAsync(int? id);
        Task DeleteTaskAsync(int? id);
        Task TransferAsync(int taskId, bool full, int to, string message);
        Task<IEnumerable<DeviceDesignTemplate>> GetDeviceDesignTemplateFromTaskAsync(int taskId);
        Task<IEnumerable<DeviceComponentsTemplate>> GetDeviceComponentsTemplatesFromTaskAsync(int taskId);
        IEnumerable<ObtainedComponent> GetObtainedComponents(int taskId);
        IEnumerable<ObtainedDesign> GetObtainedDesigns(int taskId);
        IEnumerable<LogDTO> GetLogs(int? taskId);
        string GetTaskStatusName(StatusEnum item);
        Task ReceiveComponentAsync(int taskId, int[] componentIds, int[] componentObt);
        Task ReceiveDesignAsync(int taskId, int[] designIds, int[] designObt);
        
        void Dispose();
    }
}