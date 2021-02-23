using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IDesignService
    {
        void CreateDesign(DesignDTO designDto);
        void UpdateDesign(DesignDTO designDto);
        IEnumerable<DesignDTO> GetDesigns();
        DesignDTO GetDesign(int? id);
        void DeleteDesign(int? id);
        IEnumerable<string> GetTypes();
        void AddDesign(int? id, int quantity);
        
        void Dispose();
    }
}