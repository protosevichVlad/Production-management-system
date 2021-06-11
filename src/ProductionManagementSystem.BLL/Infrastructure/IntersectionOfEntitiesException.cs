using System;

namespace ProductionManagementSystem.BLL.Infrastructure
{
    public class IntersectionOfEntitiesException : Exception
    {
    public string Header { get; protected set; }
    public IntersectionOfEntitiesException(string header, string message) : base(message)
    {
        Header = header;
    }
    }
}