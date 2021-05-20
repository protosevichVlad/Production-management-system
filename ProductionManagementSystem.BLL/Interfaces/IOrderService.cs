using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface IOrderService
    {
        Task CreateOrderAsync(OrderDTO orderDto);
        Task UpdateOrderAsync(OrderDTO orderDto);
        Task<IEnumerable<OrderDTO>> GetOrdersAsync();
        Task<IEnumerable<TaskDTO>> GetTasksFromOrderAsync(int orderId);
        Task<OrderDTO> GetOrderAsync(int? id);
        Task DeleteOrderAsync(int? id);

        void Dispose();
    }
}