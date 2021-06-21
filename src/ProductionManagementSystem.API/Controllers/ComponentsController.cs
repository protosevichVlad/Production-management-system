using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.API.Models;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductionManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        private readonly IComponentService _componentService;
        private IMapper _mapper;
        
        
        public ComponentsController(IComponentService service)
        {
            _componentService = service;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ComponentDTO, Component>();
                    cfg.CreateMap<Component, ComponentDTO>();
                })
                .CreateMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<Component>> Get()
        {
            return _mapper.Map<IEnumerable<ComponentDTO>, IEnumerable<Component>>(await _componentService.GetComponentsAsync());
        }
        
        // GET api/components/gettypes
        [HttpGet]
        [Route("gettypes")]
        public async Task<IEnumerable<string>> GetTypes()
        {
            return await _componentService.GetTypesAsync();
        }
        
        // GET api/components/add/5?quantity=10
        [HttpPost("add/{id}")]
        public async Task<ActionResult> AddComponents(int id, int? quantity)
        {
            if (quantity == null)
            {
                return BadRequest();
            }
            
            await _componentService.AddComponentAsync(id, (int)quantity);
            return Ok();
        }
        
        // GET api/components/receive/5?quantity=10
        [HttpPost("receive/{id}")]
        public async Task<ActionResult> ReceiveComponents(int id, int? quantity)
        {
            if (quantity == null)
            {
                return BadRequest();
            }
            
            await _componentService.AddComponentAsync(id, (int)-quantity);
            return Ok();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Component>> Get(int id)
        {
            return _mapper.Map<ComponentDTO, Component>(await _componentService.GetComponentAsync(id));
        }
        
        // POST api/components
        [HttpPost]
        public async Task<ActionResult<Component>> Post(Component component)
        {
            if (component == null)
            {
                return BadRequest();
            }
 
            await _componentService.CreateComponentAsync(_mapper.Map<Component, ComponentDTO>(component));
            return Ok(component);
        }
        
        // PUT api/components/
        [HttpPut]
        public async Task<ActionResult<Component>> Put(Component component)
        {
            if (component == null)
            {
                return BadRequest();
            }
            
            await _componentService.UpdateComponentAsync(_mapper.Map<Component, ComponentDTO>(component));
            return Ok(component);
        }
        
        // DELETE api/components/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Component>> Delete(int id)
        {
            var component = _mapper.Map<ComponentDTO, Component>(await _componentService.GetComponentAsync(id));
            await _componentService.DeleteComponentAsync(component.Id);
            return Ok(component);
        }
    }
}
