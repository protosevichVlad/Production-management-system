using System.Collections.Generic;
using ProductionManagementSystem.WEB.Helpers;

namespace ProductionManagementSystem.WEB.Models.Charts
{
    public class Dataset
    {
        public string Label { get; set; }
        public string Color { get; set; }
        public List<int> Data { get; set; }
        public string DataStringArray
        {
            get => ArrayHelper.ToString(Data);
        }
    }
}