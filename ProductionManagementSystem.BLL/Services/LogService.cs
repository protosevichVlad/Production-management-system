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
    public class LogService : ILogService
    {
        private IUnitOfWork _database { get; set; }
        private IMapper _mapper;
        public static string UserName;

        public LogService(IUnitOfWork uow)
        {
            _database = uow;
            _mapper = new MapperConfiguration(cnf =>
            {
                cnf.CreateMap<Log, LogDTO>();
                cnf.CreateMap<LogDTO, Log>();
            }).CreateMapper();
        }
        

        public void CreateLog(LogDTO logDto)
        {
            var log = _mapper.Map<LogDTO, Log>(logDto);
            log.UserLogin = UserName;
            _database.Logs.Create(log);
            _database.Save();
        }

        public IEnumerable<LogDTO> GetLogs()
        {
            return _mapper.Map<IEnumerable<Log>, IEnumerable<LogDTO>>(_database.Logs.GetAll().Reverse());
        }

        public LogDTO GetLog(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            var log = _mapper.Map<Log, LogDTO>(_database.Logs.Get((int) id));
            if (log == null)
            {
                throw new PageNotFoundException();
            }

            return log;
        }
        
        public void Dispose()
        {
            _database.Dispose();
        }
    }
}