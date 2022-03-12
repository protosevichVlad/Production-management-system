using System.Collections.Generic;
using System.Linq;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    public class BaseAltiumDbEntity : Dictionary<string, string>
    {
        public BaseAltiumDbEntity()
        {
            foreach (var columnName in Fields)
            {
                this[columnName] = "";
            }
        }
        
        public string Item
        {
            get => this["Item"];
            set => this["Item"] = value;
        }
        
        public string PartNumber
        {
            get => this["Part Number"];
            set => this["Part Number"] = value;
        }
        
        public string LibraryRef
        {
            get => this["Library Ref"];
            set => this["Library Ref"] = value;
        }
        
        public string FootprintRef
        {
            get => this["Footprint Ref"];
            set => this["Footprint Ref"] = value;
        }
        
        public string LibraryPath // заполняется для всей таблицы
        {
            get => this["Library Path"];
            set => this["Library Path"] = value;
        }
        
        public string FootprintPath // заполняется для всей таблицы
        {
            get => this["Footprint Path"];
            set => this["Footprint Path"] = value;
        }
        
        public string Supplier
        {
            get => this["Supplier"];
            set => this["Supplier"] = value;
        }
        
        public string Case
        {
            get => this["Case"];
            set => this["Case"] = value;
        }
        
        public string Manufacturer
        {
            get => this["Manufacturer"];
            set => this["Manufacturer"] = value;
        }
        
        public string HelpURL
        {
            get => this["HelpURL"];
            set => this["HelpURL"] = value;
        }
        
        public string Description
        {
            get => this["Description"];
            set => this["Description"] = value;
        }
        
        public string Category
        {
            get => this["Category"];
            set => this["Category"] = value;
        }
        
        public string ImageLink
        {
            get => this["ImageLink"];
            set => this["ImageLink"] = value;
        }

        public static List<string> Fields { get; } = new List<string>()
        {
            "Item", "Part Number", "Library Ref", "Footprint Ref", "Library Path", "Footprint Path", "Supplier",
            "Case", "Manufacturer", "HelpURL", "Description", "Category", "ImageLink"
        };
        
        public static List<string> ImportantFields { get; } = new List<string>()
        {
            "Item", "Manufacturer", "HelpURL", "Description"
        };

        public static List<string> NoImportantFields { get; } = Fields.Where(x => !ImportantFields.Contains(x) && x != "Part Number" && x != "ImageLink").ToList();
    }
}