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
        IEnumerable<ObtainedComponentDTO> GetObtainedComponents(int taskId);
        IEnumerable<ObtainedDesignDTO> GetObtainedDesigns(int taskId);
        IEnumerable<LogDTO> GetLogs(int? taskId);
        string GetTaskStatusName(StatusEnum item);
        Task ReceiveComponentsAsync(int taskId, int[] componentIds, int[] componentObt);
        Task ReceiveDesignsAsync(int taskId, int[] designIds, int[] designObt);
        Task ReceiveComponentAsync(int taskId, int componentId, int componentObt);
        Task ReceiveDesignAsync(int taskId, int designId, int designObt);
        
        void Dispose();
    }
}