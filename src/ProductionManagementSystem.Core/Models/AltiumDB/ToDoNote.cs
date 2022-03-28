using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("AltiumDB_ToDoNotes")]
    public class ToDoNote
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public bool Completed { get; set; }
        public string Description { get; set; }
    }
}