using System;

namespace ProductionManagementSystem.BLL.Infrastructure
{
    public class IntersectionOfEntitiesException : Exception
    {
    public string Header { get; }
    public IntersectionOfEntitiesException(string header, string message) : base(message)
    {
        Header = header;
    }
    }
}