using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.AltiumDB.Projects
{
    [Table("AltiumDB_Projects")]
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<EntityInProject> Entities { get; set; }
        public string ImagePath { get; set; }
        public string BOMFilePath { get; set; }
        public string CircuitDiagramPath { get; set; }
        public string AssemblyDrawingPath { get; set; }
        public string ThreeDModelPath { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        
        public DateTime ReportDate { get; set; }
        public string Variant { get; set; }

        public override string ToString()
        {
            return $"{this.Name} {this.Variant}";
        }
    }
}