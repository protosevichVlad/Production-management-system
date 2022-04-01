using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.WEB.Models.AltiumDB.GlobalSearch
{
    public class GlobalSearchHintsSectionViewModel
    {
        public string Name { get; set; }
        public List<GlobalSearchHintsSectionRowViewModel> Rows { get; set; }
    }
}