using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;

namespace ProductionManagementSystem.BLL.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork _database { get; }
        private ITaskService _taskService; 
        private IMapper _mapper;
        
        public OrderService(IUnitOfWork uow)
        {
            _database = uow;
            _taskService = new TaskService(uow);
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>()
                    .ForMember(o => o.Status, opt => opt.MapFrom(
                        o => _taskService.GetTaskStatusName(o.Tasks.Min(t => t.Status))));
                cfg.CreateMap<OrderDTO, Order>();
                cfg.CreateMap<Task, TaskDTO>();
                cfg.CreateMap<TaskDTO, Task>();
                cfg.CreateMap<Device, DeviceDTO>();
                cfg.CreateMap<DeviceDTO, Device>();
            }).CreateMapper();

        }
        
        public void CreateOrder(OrderDTO orderDto)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDto);

            order.DateStart = DateTime.Now.Date;
            order.Tasks = new List<Task>();
            
            for (int i = 0; i < orderDto.DeviceIds.Length; i++)
            {
                for (int j = 0; j < orderDto.DeviceQuantity[i]; j++)
                {
                    new TaskService(_database).CreateTask(new TaskDTO()
                    {
                        Deadline = orderDto.Deadline,
                        Description = orderDto.DeviceDescriptions[i],
                        DeviceId = orderDto.DeviceIds[i],
                    });

                    var task = _database.Tasks.GetAll().LastOrDefault();
                    if (task != null)
                    {
                        order.Tasks.Add(task);
                    }
                }
            }
            
            _database.Orders.Create(order);
            _database.Save();
        }

        public void UpdateOrder(OrderDTO orderDto)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDto);
            _database.Orders.Update(order);
            _database.Save();
        }

        public IEnumerable<OrderDTO> GetOrders()
        {
            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(_database.Orders.GetAll());
        }

        public IEnumerable<TaskDTO> GetTasksFromOrder(int orderId)
        {
            var tasks = GetOrder(orderId).Tasks;
            return tasks;
        }

        public OrderDTO GetOrder(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            return _mapper.Map<Order, OrderDTO>(_database.Orders.Get((int) id));
        }

        public void DeleteOrder(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }

            var order = GetOrder(id);

            foreach (var task in order.Tasks)
            {
                _database.Tasks.Delete(task.Id);
                _database.Save();
            }
            
            _database.Orders.Delete((int) id);
            _database.Save();
        }

        public void Dispose()
        {
            _database.Dispose();
        }
    }
}