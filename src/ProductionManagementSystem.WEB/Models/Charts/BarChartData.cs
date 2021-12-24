using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.ElementsDifference;

namespace ProductionManagementSystem.WEB.Models.Charts
{
    public class BarChartData : BaseChart
    {
        public BarChartData() { }
        
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public string Label { get; set; }
    }
}