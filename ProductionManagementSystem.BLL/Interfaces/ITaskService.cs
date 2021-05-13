using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ITaskService
    {
        void CreateTask(TaskDTO taskDto);
        void UpdateTask(TaskDTO taskDto);
        void EditTask(TaskDTO taskDto);
        IEnumerable<TaskDTO> GetTasks();
        IEnumerable<TaskDTO> GetTasks(IEnumerable<string> roles);
        TaskDTO GetTask(int? id);
        void DeleteTask(int? id);
        void Transfer(int taskId, bool full, int to, string message);
        IEnumerable<DeviceDesignTemplate> GetDeviceDesignTemplateFromTask(int taskId);
        IEnumerable<DeviceComponentsTemplate> GetDeviceComponentsTemplatesFromTask(int taskId);
        IEnumerable<ObtainedComponent> GetObtainedComponents(int taskId);
        IEnumerable<ObtainedDesign> GetObtainedDesigns(int taskId);
        IEnumerable<LogDTO> GetLogs(int? taskId);
        string GetTaskStatusName(StatusEnum item);
        void ReceiveComponent(int taskId, int[] componentIds, int[] componentObt);
        void ReceiveDesign(int taskId, int[] designIds, int[] designObt);
        
        void Dispose();
    }
}