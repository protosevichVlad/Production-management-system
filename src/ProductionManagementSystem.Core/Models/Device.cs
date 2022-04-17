using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductionManagementSystem.Core.Models
{
   [Table("CompDB_Devices")]
   public class CompDbDevice
   {
      [Key]
      public int Id { get; set; }
      public string Name { get; set; }
      public string Variant { get; set; }
      public string Description { get; set; }
      
      public string ImagePath { get; set; }
      public DateTime ReportDate { get; set; }
      public int Quantity { get; set; }
      
      public List<UsedInDevice> UsedInDevice { get; set; }
   }

   [Table("UsedInDevice")]
   public class UsedInDevice
   {
      [Key]
      public int Id { get; set; }
      public int UsedComponentId { get; set; }
      public UsedInDeviceComponentType ComponentType { get; set; }
      [NotMapped]
      public object Component { get; set; }
      public int Quantity { get; set; }
      public string Designator { get; set; }
      public int CompDbDeviceId { get; set; }
      public CompDbDevice CompDbDevice { get; set; }
   }

   public enum UsedInDeviceComponentType
   {
      Entity = 1,
      PCB = 2,
      Device = 3,
   }
}