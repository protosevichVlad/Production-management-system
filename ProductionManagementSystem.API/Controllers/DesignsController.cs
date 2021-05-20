using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.API.Models;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Interfaces;

namespace ProductionManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DesignsController : Controller
    {
        private readonly IDesignService _designService;
        private readonly IDeviceService _deviceService;
        private IMapper _mapper;
        
        
        public DesignsController(IDesignService service, IDeviceService deviceService)
        {
            _deviceService = deviceService;
            _designService = service;
            _mapper = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<DesignDTO, Design>();
                    cfg.CreateMap<Design, DesignDTO>();
                })
                .CreateMapper();
        }

        [HttpGet]
        public async Task<IEnumerable<Design>> Get()
        {
            return _mapper.Map<IEnumerable<DesignDTO>, IEnumerable<Design>>(await _designService.GetDesignsAsync());
        }
        
        // GET api/designs/gettypes
        [HttpGet]
        [Route("gettypes")]
        public async Task<IEnumerable<string>> GetTypes()
        {
            return await _designService.GetTypesAsync();
        }
        
        // GET api/designs/add/5?quantity=10
        [HttpPost("add/{id}")]
        public async Task<ActionResult> AddDesigns(int id, int? quantity)
        {
            if (quantity == null)
            {
                return BadRequest();
            }
            
            await _designService.AddDesignAsync(id, (int)quantity);
            return Ok();
        }
        
        // GET api/designs/receive/5?quantity=10
        [HttpPost("add/{id}")]
        public async Task<ActionResult> ReceiveDesigns(int id, int? quantity)
        {
            if (quantity == null)
            {
                return BadRequest();
            }
            
            await _designService.AddDesignAsync(id, (int)-quantity);
            return Ok();
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Design>> Get(int id)
        {
            return _mapper.Map<DesignDTO, Design>(await _designService.GetDesignAsync(id));
        }
        
        // POST api/designs
        [HttpPost]
        public async Task<ActionResult<Design>> Post(Design design)
        {
            if (design == null)
            {
                return BadRequest();
            }
 
            await _designService.CreateDesignAsync(_mapper.Map<Design, DesignDTO>(design));
            return Ok(design);
        }
        
        // PUT api/designs/
        [HttpPut]
        public async Task<ActionResult<Design>> Put(Design design)
        {
            if (design == null)
            {
                return BadRequest();
            }
            
            await _designService.UpdateDesignAsync(_mapper.Map<Design, DesignDTO>(design));
            return Ok(design);
        }
        
        // DELETE api/designs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Design>> Delete(int id)
        {
            var design = _mapper.Map<DesignDTO, Design>(await _designService.GetDesignAsync(id));
            await _designService.DeleteDesignAsync(design.Id);
            return Ok(design);
        }
    }
}