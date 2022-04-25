using System.Collections.Generic;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class DataListViewModel<TItem>
    {
        public List<FilterViewModel> Filters { get; set; }
        public List<TItem> Data { get; set; }
        public PaginationViewModel Pagination { get; set; }
    }
    
    public class EntityDataListViewModel : DataListViewModel<EntityExt>
    {
        public Table Table { get; set; }
    }
}