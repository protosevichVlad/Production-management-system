using System;
using System.Collections.Generic;
using ProductionManagementSystem.BLL.DTO;

namespace ProductionManagementSystem.BLL.Interfaces
{
    public interface ILogService
    {
        void CreateLog(LogDTO logDto);
        IEnumerable<LogDTO> GetLogs();
        LogDTO GetLog(int? id);

        void Dispose();
    }
}