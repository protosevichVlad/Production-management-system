using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class EntityViewModel
    {
        public DatabaseTable Table { get; set; }
        public BaseAltiumDbEntity Data { get; set; }
    }
}