using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Razor.Language;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.WEB.Helpers;

namespace ProductionManagementSystem.WEB.Models.Charts
{
    public class StackedBarChart
    {
        private readonly List<string> _labels = new List<string>();
        private readonly Dictionary<int, Dataset> _datasets = new Dictionary<int, Dataset>();

        public StackedBarChart(List<ElementDifference> elementDifferences, string dateTimeFormat)
        {
            Random random = new Random();
            _labels = elementDifferences.Select(x => x.DateTime.ToString(dateTimeFormat)).Distinct().ToList();
            foreach (var elementDifference in elementDifferences)
            {
                if (!_datasets.ContainsKey(elementDifference.ElementId))
                {
                    _datasets[elementDifference.ElementId] = new Dataset()
                    {
                        Color = $"rgb({random.Next(255)},{random.Next(255)},{random.Next(255)})", 
                        Data = new List<int>(Enumerable.Repeat(0, _labels.Count)), 
                        Label = $"componentId = {elementDifference.ElementId}"
                    };
                }

                var index = _labels.FindIndex(x => x == elementDifference.DateTime.ToString(dateTimeFormat));
                _datasets[elementDifference.ElementId].Data[index] += elementDifference.Difference;
            }
        }

        public string Labels 
        {
            get => ArrayHelper.ToString(_labels);
        }
        
        public List<Dataset> Datasets 
        {
            get => _datasets.Select(x => x.Value).ToList();
        }
    }
}