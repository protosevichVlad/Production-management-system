using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Services;

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
      
      [NotMapped]
      public List<UsedItem> UsedItems { get; set; }
      public string ThreeDModelPath { get; set; }
   }

   public class CreateEditDevice
   {
      public int Id { get; set; }
      public string Name { get; set; }
      public string Variant { get; set; }
      public string Description { get; set; }
      
      public byte[] ThreeDModel { get; set; }
      public byte[] Image { get; set; }
      public DateTime ReportDate { get; set; }
      public int Quantity { get; set; }
      
      public List<UsedItem> UsedItems { get; set; }

      internal async Task<CompDbDevice> GetDevice(IFileService fileService)
      {
         var pcb = new CompDbDevice()
         {
            Id = this.Id,
            Name = this.Name,
            Description = this.Description,
            Quantity = this.Quantity,
            ReportDate = this.ReportDate,
            Variant = this.Variant,
            UsedItems = this.UsedItems
         };

         pcb.ImagePath = await fileService.CreateFileByUrl(Image, $"/uploads/{Name}/{Variant}", "Image.png");
         pcb.ThreeDModelPath = await fileService.CreateFileByUrl(ThreeDModel, $"/uploads/{Name}/{Variant}", "ThreeDModel.png");
         return pcb;
      }
   }
}