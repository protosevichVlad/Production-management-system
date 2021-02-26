using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ITaskService
    {
        void CreateTask(TaskDTO taskDto);
        void UpdateTask(TaskDTO taskDto);
        void EditTask(TaskDTO taskDto);
        IEnumerable<TaskDTO> GetTasks();
        TaskDTO GetTask(int? id);
        void DeleteTask(int? id);
        void Transfer(int taskId, bool full, int to, string message);
        IEnumerable<DeviceDesignTemplate> GetDeviceDesignTemplateFromTask(int taskId);
        IEnumerable<DeviceComponentsTemplate> GetDeviceComponentsTemplatesFromTask(int taskId);
        IEnumerable<ObtainedComponent> GetObtainedСomponents(int taskId);
        IEnumerable<ObtainedDesign> GetObtainedDesigns(int taskId);
        void ReceiveComponent(int taskId, int[] componentIds, int[] componentObt);
        void ReceiveDesign(int taskId, int[] designIds, int[] designObt);
        void Dispose();
    }
}