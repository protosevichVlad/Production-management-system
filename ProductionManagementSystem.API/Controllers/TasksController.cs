using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.API.Models;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;
using ObtainedDesign = ProductionManagementSystem.API.Models.ObtainedDesign;
using Task = ProductionManagementSystem.API.Models.Task;

namespace ProductionManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITaskService _taskService;
        private IMapper _mapper;
        
        
        public TasksController(ITaskService service)
        {
            _taskService = service;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<TaskDTO, Task>();
                    cfg.CreateMap<Task, TaskDTO>();
                    cfg.CreateMap<ObtainedDesign, ObtainedDesignDTO>();
                    cfg.CreateMap<ObtainedDesignDTO, ObtainedDesign>();
                    cfg.CreateMap<ObtainedComponentDTO, ObtainedComponent>();
                    cfg.CreateMap<ObtainedComponent, ObtainedComponentDTO>();
                    cfg.CreateMap<ComponentDTO, Component>();
                    cfg.CreateMap<Component, ComponentDTO>();
                    cfg.CreateMap<DesignDTO, Design>();
                    cfg.CreateMap<Design, DesignDTO>();
                })
                .CreateMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<Task>> Get()
        {
            return _mapper.Map<IEnumerable<TaskDTO>, IEnumerable<Task>>(await _taskService.GetTasksAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Task>> Get(int id)
        {
            return _mapper.Map<TaskDTO, Task>(await _taskService.GetTaskAsync(id));
        }
        
        // POST api/designs
        [HttpPost]
        public async Task<ActionResult<Task>> Post(Task task)
        {
            if (task == null)
            {
                return BadRequest();
            }
 
            await _taskService.CreateTaskAsync(_mapper.Map<Task, TaskDTO>(task));
            return Ok(task);
        }
        
        // PUT api/designs/
        [HttpPut]
        public async Task<ActionResult<Task>> Put(Task task)
        {
            if (task == null)
            {
                return BadRequest();
            }
            
            await _taskService.UpdateTaskAsync(_mapper.Map<Task, TaskDTO>(task));
            return Ok(task);
        }
        
        // DELETE api/designs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Task>> Delete(int id)
        {
            var task = _mapper.Map<TaskDTO, Task>(await _taskService.GetTaskAsync(id));
            await _taskService.DeleteTaskAsync(task.Id);
            return Ok(task);
        }
    }
}