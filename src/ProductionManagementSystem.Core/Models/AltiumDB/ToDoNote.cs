using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Users;

namespace ProductionManagementSystem.Core.Models.AltiumDB
{
    [Table("AltiumDB_ToDoNotes")]
    public class ToDoNote
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime CompletedDateTime { get; set; }
        public bool Completed { get; set; }
        public string Description { get; set; }
        public User CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public User CompletedBy { get; set; }
        public string CompletedById { get; set; }
    }
}