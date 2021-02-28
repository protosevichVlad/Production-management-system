using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IOrderService
    {
        void CreateOrder(OrderDTO orderDto);
        void UpdateOrder(OrderDTO orderDto);
        IEnumerable<OrderDTO> GetOrders();
        IEnumerable<TaskDTO> GetTasksFromOrder(int orderId);
        OrderDTO GetOrder(int? id);
        void DeleteOrder(int? id);

        void Dispose();
    }
}