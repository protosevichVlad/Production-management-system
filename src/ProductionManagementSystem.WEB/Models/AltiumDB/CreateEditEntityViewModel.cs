using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class CreateEditEntityViewModel
    {
        public DatabaseTable Table { get; set; }
        public IDictionary<string, object> Data { get; set; }
    }
}