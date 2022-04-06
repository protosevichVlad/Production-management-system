using System.Collections.Generic;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class EntityViewModel
    {
        public Table Table { get; set; }
        public AltiumDbEntity Data { get; set; }
    }
}