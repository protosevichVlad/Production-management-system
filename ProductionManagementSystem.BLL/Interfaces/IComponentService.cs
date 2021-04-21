using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IComponentService
    {
        void CreateComponent(ComponentDTO componentDto);
        void UpdateComponent(ComponentDTO componentDto);
        IEnumerable<ComponentDTO> GetComponents();
        ComponentDTO GetComponent(int? id);
        void DeleteComponent(int? id);
        IEnumerable<string> GetTypes();
        void AddComponent(int? id, int quantity);
    
        void Dispose();
    }
}