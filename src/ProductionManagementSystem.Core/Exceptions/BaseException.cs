using System;

namespace ProductionManagementSystem.Core.Exceptions
{
    public class BaseException : Exception
    {
        public BaseException(string message) : base(message)
        {
            
        }
    }
}