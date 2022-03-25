using System;
using System.Collections.Generic;

namespace ProductionManagementSystem.WEB.Models.AltiumDB
{
    public class FilterViewModel
    {
        public string FilterName { get; set; }
        public List<(string value, bool selected)> Values { get; set; }
    }
}