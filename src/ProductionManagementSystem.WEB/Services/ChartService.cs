using System.Collections.Generic;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.WEB.Models.Charts;

namespace ProductionManagementSystem.WEB.Services
{
    public static class ChartService
    {
        public static BarChartData ElementDiffToBarChart(List<ElementDifference> elementDifferences)
        {
            BarChartData result = new BarChartData();
            result.Data = new List<int>();
            result.Labels = new List<string>();
            foreach (var elementDifference in elementDifferences)
            {
                result.Data.Add(elementDifference.Difference);
                result.Labels.Add(elementDifference.DateTime.Date.ToString("dd.MM.yyyy"));
            }

            return result;
        }
    }
}