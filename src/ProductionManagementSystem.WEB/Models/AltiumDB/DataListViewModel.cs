using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class DataListViewModel
    {
        public List<FilterViewModel> Filters { get; set; }
        public DatabaseTable DatabaseTable { get; set; }
        public List<BaseAltiumDbEntity> Data { get; set; }
    }
}