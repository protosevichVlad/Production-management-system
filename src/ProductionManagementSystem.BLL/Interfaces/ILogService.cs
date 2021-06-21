using System.Collections.Generic;
using System.Threading.Tasks;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ILogService
    {
        Task CreateLogAsync(LogDTO logDto);
        Task<IEnumerable<LogDTO>> GetLogsAsync();
        Task<LogDTO> GetLogAsync(int? id);

        void Dispose();
    }
}