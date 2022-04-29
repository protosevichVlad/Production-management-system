using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ProductionManagementSystem.Core.Models.Tasks;

namespace ProductionManagementSystem.Core.Models
{
    [Table("CDBTasks")]
    public class CDBTask
    {
        public int Id { get; set; }
        public int ParentTaskId { get; set; }
        public int ItemId { get; set; }
        [NotMapped]
        public UniversalItem Item { get; set; }
        public TaskStatusEnum Status { get; set; } = TaskStatusEnum.Equipment;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public List<CDBObtained> Obtained { get; set; }
        public CDBItemType ItemType { get; set; }
    }

    public enum CDBItemType
    {
        Entity,
        PCB,
        Device
    }

    [Table("CDBObtained")]
    public class CDBObtained
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int TaskId { get; set; }
        public CDBTask Task { get; set; }
        public int UsedItemId { get; set; }
        public UsedItem UsedItem { get; set; }
    }
}