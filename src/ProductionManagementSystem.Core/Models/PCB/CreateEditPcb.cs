using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ProductionManagementSystem.Core.Services;

namespace ProductionManagementSystem.Core.Models.PCB
{
    public class CreateEditPcb
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<UsedItem> UsedItems { get; set; }
        public byte[] Image { get; set; }
        public byte[] CircuitDiagram { get; set; }
        public byte[] AssemblyDrawing { get; set; }
        public byte[] ThreeDModel { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        
        public DateTime ReportDate { get; set; }
        public string Variant { get; set; }

        public override string ToString()
        {
            return $"{this.Name} {this.Variant}";
        }


        public async Task<Pcb> GetPcb(IFileService fileService)
        {
            var pcb = new Pcb()
            {
                Id = this.Id,
                Name = this.Name,
                UsedItems = this.UsedItems,
                Description = this.Description,
                Quantity = this.Quantity,
                ReportDate = this.ReportDate,
                Variant = this.Variant,
            };

            pcb.ImagePath = await  fileService.CreateFileByUrl(Image, $"/uploads/{Name}/{Variant}", "Image.png");
            pcb.CircuitDiagramPath = await  fileService.CreateFileByUrl(CircuitDiagram, $"/uploads/{Name}/{Variant}", "CircuitDiagram.pdf");
            pcb.AssemblyDrawingPath = await  fileService.CreateFileByUrl(AssemblyDrawing, $"/uploads/{Name}/{Variant}", "AssemblyDrawing.pdf");
            pcb.ThreeDModelPath = await  fileService.CreateFileByUrl(ThreeDModel, $"/uploads/{Name}/{Variant}", "ThreeDModel.stl");
            return pcb;
        }
    }
}