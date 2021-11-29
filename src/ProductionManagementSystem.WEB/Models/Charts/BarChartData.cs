using System.Collections.Generic;

namespace ProductionManagementSystem.WEB.Models.Charts
{
    public class BarChartData
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public string Label { get; set; }
    }
}