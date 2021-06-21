using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.DAL.Entities;

namespace BLLTests
{
    public class ComponentsServiceTestsSource
    {
        public static List<Component> Components = new List<Component>
        {
            new Component
            {
                Id = 0,
                Name = "asd",
                Quantity = 12
            }
        };
        
        public static List<ComponentDTO> ComponentsDto = new List<ComponentDTO>
        {
            new ComponentDTO
            {
                Id = 0,
                Name = "asd",
                Quantity = 12
            }
        };
    }
}