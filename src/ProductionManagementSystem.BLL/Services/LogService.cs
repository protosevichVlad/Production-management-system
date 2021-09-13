using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ProductionManagementSystem.BLL.DTO;
using ProductionManagementSystem.BLL.Infrastructure;
using ProductionManagementSystem.BLL.Interfaces;
using ProductionManagementSystem.DAL.Entities;
using ProductionManagementSystem.DAL.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace ProductionManagementSystem.BLL.Services
{
    public class LogService : ILogService
    {
        private readonly IUnitOfWork _database;
        private readonly IMapper _mapper;
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
        

        public async Task CreateLogAsync(LogDTO logDto)
        {
            var log = _mapper.Map<LogDTO, Log>(logDto);
            log.UserLogin = UserName;
            await _database.Logs.CreateAsync(log);
            await _database.SaveAsync();
        }

        public async Task<IEnumerable<LogDTO>> GetLogsAsync()
        {
            return _mapper.Map<IEnumerable<Log>, IEnumerable<LogDTO>>((await _database.Logs.GetAllAsync()).Reverse());
        }

        public async Task<LogDTO> GetLogAsync(int? id)
        {
            if (id == null)
            {
                throw new PageNotFoundException();
            }
            
            var log = _mapper.Map<Log, LogDTO>(await _database.Logs.GetAsync((int) id));
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