﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Enums;
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

        public async Task<IActionResult> Index(string userName, int? deviceId, int? componentId, int? designId, int? taskId, int? orderId)
        {
            var logs = _mapper.Map<IEnumerable<LogDTO>, IEnumerable<LogViewModel>>(await _logService.GetLogsAsync());

            if (userName != null)
            {
                logs = logs.Where(l => l.UserLogin == userName);
            }
            
            if (deviceId != null)
            {
                logs = logs.Where(l => l.DesignId == deviceId);
            }
            
            if (componentId != null)
            {
                logs = logs.Where(l => l.ComponentId == componentId);
            }
            
            if (designId != null)
            {
                logs = logs.Where(l => l.DesignId == designId);
            }
            
            if (taskId != null)
            {
                logs = logs.Where(l => l.TaskId == taskId);
            }
            
            if (orderId != null)
            {
                logs = logs.Where(l => l.OrderId == orderId);
            }
            
            return View(logs);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                return View(_mapper.Map<LogDTO, LogViewModel>(await _logService.GetLogAsync(id)));
            }
            catch (PageNotFoundException)
            {
                throw new Exception("Страница не найдена.");
            }
        }
    }
}