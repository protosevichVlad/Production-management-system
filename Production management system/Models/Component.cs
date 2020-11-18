using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.Models
{
    public class Component
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Nominal { get; set; }
        public string Corpus { get; set; }
        public string Explanation { get; set; }
        public string Manufacturer { get; set; }

        public int Quantity { get; set; }
    }
}
