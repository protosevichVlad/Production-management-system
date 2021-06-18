using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Enums;
using ProductionManagementSystem.DAL.Interfaces;
using Task = ProductionManagementSystem.DAL.Entities.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _database;
        private readonly ITaskService _taskService; 
        private readonly IMapper _mapper;
        
        public OrderService(IUnitOfWork uow)
        {
            _database = uow;
            _taskService = new TaskService(uow);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>()
                    .ForMember(
                            order => order.Status,
                            opt => opt.MapFrom(
                                src => _taskService.GetTaskStatusName(StatusEnum.Assembly)
                                )
                            );
                cfg.CreateMap<OrderDTO, Order>();
                cfg.CreateMap<Task, TaskDTO>();
                cfg.CreateMap<TaskDTO, Task>();
                cfg.CreateMap<Device, DeviceDTO>();
                cfg.CreateMap<DeviceDTO, Device>();
            }).CreateMapper();

        }
        
        public async System.Threading.Tasks.Task CreateOrderAsync(OrderDTO orderDto)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDto);

            order.DateStart = DateTime.Now.Date;
            order.Tasks = new List<Task>();
            
            for (int i = 0; i < orderDto.DeviceIds.Length; i++)
            {
                for (int j = 0; j < orderDto.DeviceQuantity[i]; j++)
                {
                    await new TaskService(_database).CreateTaskAsync(new TaskDTO()
                    {
                        Deadline = orderDto.Deadline,
                        Description = orderDto.DeviceDescriptions[i],
                        DeviceId = orderDto.DeviceIds[i],
                    });

                    var task = (await _database.Tasks.GetAllAsync()).LastOrDefault();
                    if (task != null)
                    {
                        order.Tasks.Add(task);
                    }
                }
            }
            
            await _database.Orders.CreateAsync(order);
            await _database.SaveAsync();
        }

        public async System.Threading.Tasks.Task UpdateOrderAsync(OrderDTO orderDto)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDto);
            _database.Orders.Update(order);
            await _database.SaveAsync();
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersAsync()
        {
            var orders = _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(await _database.Orders.GetAllAsync()).ToList();
            foreach (var order in orders)
            {
                order.Status = _taskService.GetTaskStatusName(order.Tasks.Min(t => t.Status));
            }

            return orders;
        }

        public async Task<IEnumerable<TaskDTO>> GetTasksFromOrderAsync(int orderId)
        {
            var tasks = (await GetOrderAsync(orderId)).Tasks;
            return tasks;
        }

        public async Task<OrderDTO> GetOrderAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            return _mapper.Map<Order, OrderDTO>(await _database.Orders.GetAsync((int) id));
        }

        public async System.Threading.Tasks.Task DeleteOrderAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var order = await GetOrderAsync(id);

            foreach (var task in order.Tasks)
            {
                await _database.Tasks.DeleteAsync(task.Id);
                await _database.SaveAsync();
            }
            
            await _database.Orders.DeleteAsync((int) id);
            await _database.SaveAsync();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}