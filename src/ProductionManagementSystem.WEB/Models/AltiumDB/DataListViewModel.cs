using System.Collections.Generic;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class DataListViewModel
    {
        public List<FilterViewModel> Filters { get; set; }
        public Table Table { get; set; }
        public List<EntityExt> Data { get; set; }
        public PaginationViewModel Pagination { get; set; }
    }
}