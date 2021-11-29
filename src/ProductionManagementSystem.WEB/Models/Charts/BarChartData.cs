using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.ElementsDifference;

namespace ProductionManagementSystem.WEB.Models.Charts
{
    public class BarChartData
    {
        public BarChartData(List<ElementDifference> elementDifferences)
        {
            Data = new List<int>();
            Labels = new List<string>();
            foreach (var elementDifference in elementDifferences)
            {
                Data.Add(elementDifference.Difference);
                Labels.Add(elementDifference.DateTime.Date.ToString("dd.MM.yyyy"));
            }
        }
        
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
        public string Label { get; set; }
    }
}