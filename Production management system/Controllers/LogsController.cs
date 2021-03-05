using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.Models;
using ProductionManagementSystem.WEB.Models;

namespace ProductionManagementSystem.Controllers
{
    [Authorize(Roles = RoleEnum.Admin)]
    public class LogsController : Controller
    {
        private ILogService _logService;
        private IMapper _mapper;

        public LogsController(ILogService service)
        {
            _logService = service;
            _mapper = new MapperConfiguration(cnf =>
            {
                cnf.CreateMap<LogDTO, LogViewModel>();
                cnf.CreateMap<LogViewModel, LogDTO>();
            }).CreateMapper();
        }

        public IActionResult Index()
        {
            var logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<LogViewModel>>(_logService.GetLogs());
            return View(logs);
        }
        
        public IActionResult Details(int? id)
        {
            try
            {
                return View(_mapper.Map<LogDTO, LogViewModel>(_logService.GetLog(id)));
            }
            catch (PageNotFoundException)
            {
                return NotFound();
            }
        }
    }
}