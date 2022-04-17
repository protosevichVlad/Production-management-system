using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Models.PCB;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;
using Directory = System.IO.Directory;

namespace ProductionManagementSystem.Core.Services
{
    public interface IPcbService : IBaseService<Pcb>
    {
        Task<Pcb> ImportPcbAsync(Stream bom, Stream image, Stream circuitDiagram,
            Stream assemblyDrawing, Stream threeDModel);

        Task<List<Pcb>> GetPcbWithEntityAsync(string partNumber);
        Task<List<Pcb>> SearchByKeyWordAsync(string keyWord);
        Task DeleteByIdAsync(int id);
        Task IncreaseQuantityAsync(int id, int quantity);
        Task DecreaseQuantityAsync(int id, int quantity);
        Task ChangeQuantityAsync(int id, int quantity);
    }
    public class PcbService : BaseService<Pcb, IUnitOfWork>, IPcbService
    {
        private readonly IEntityExtService _entityExtService;
        
        public PcbService(IUnitOfWork db, IEntityExtService entityService) : base(db)
        {
            _entityExtService = entityService;
            _currentRepository = _db.Pcbs;
        }

        public async Task<Pcb> ImportPcbAsync(Stream bom, Stream image, Stream circuitDiagram,
            Stream assemblyDrawing, Stream threeDModel)
        {
            Pcb pcb = new Pcb();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Stream bomExcel = new MemoryStream();
            await bom.CopyToAsync(bomExcel);
            using(var package = new ExcelPackage(bomExcel))
            {
                var sheet = package.Workbook.Worksheets[0];
                pcb.Name = sheet.Cells["D4"].Text;
                if (pcb.Name.EndsWith(".PrjPcb"))
                {
                    pcb.Name = pcb.Name.Substring(0, pcb.Name.Length - ".PrjPcb".Length);
                }
                
                pcb.Variant = sheet.Cells["D5"].Text;

                if (DateTime.TryParseExact(sheet.Cells["D7"].Text + "_" + sheet.Cells["E7"].Text, "dd.MM.yyyy_hh:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var projectReportDate))
                    pcb.ReportDate = projectReportDate;
                else
                    pcb.ReportDate = DateTime.Now;
                
                pcb.Entities = new List<EntityInPcb>();
                int row = 10;
                while (true)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[row, 6].Text))
                        break;
                    
                    EntityInPcb entityInPcb = new EntityInPcb();
                    entityInPcb.Designator = sheet.Cells[row, 4].Text;
                    entityInPcb.EntityId =
                        (await _entityExtService.GetEntityExtByPartNumber(sheet.Cells[row, 6].Text))?.KeyId  ?? 0;
                    entityInPcb.Quantity = int.Parse(sheet.Cells[row, 10].Text);

                    pcb.Entities.Add(entityInPcb);
                    row++;
                }
            }
            
            string basePath = Path.Combine("wwwroot", "uploads", pcb.Name, pcb.Variant);
            string wwwPath = Path.Combine("uploads", pcb.Name, pcb.Variant);
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            pcb.BOMFilePath = '/' + Path.Combine(wwwPath, $"{pcb.Name}_{pcb.Variant}_bom.xlsx").Replace('\\', '/');
            pcb.ImagePath = '/' + Path.Combine(wwwPath, $"{pcb.Name}_{pcb.Variant}_image.png").Replace('\\', '/');
            pcb.CircuitDiagramPath = '/' + Path.Combine(wwwPath, $"{pcb.Name}_{pcb.Variant}_circuitDiagram.pdf").Replace('\\', '/');
            pcb.AssemblyDrawingPath = '/' + Path.Combine(wwwPath, $"{pcb.Name}_{pcb.Variant}_assemblyDrawing.pdf").Replace('\\', '/');
            pcb.ThreeDModelPath = '/' + Path.Combine(wwwPath, $"{pcb.Name}_{pcb.Variant}_3dModel.stl").Replace('\\', '/');
            pcb.Description = "This project was created with using a BOM file";
            pcb.Quantity = 0;
            
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{pcb.Name}_{pcb.Variant}_bom.xlsx"), FileMode.Create)) {  
                await bom.CopyToAsync(outputFileStream);  
            }
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{pcb.Name}_{pcb.Variant}_image.png"), FileMode.Create)) {  
                await image.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{pcb.Name}_{pcb.Variant}_circuitDiagram.pdf"), FileMode.Create)) {  
                await circuitDiagram.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{pcb.Name}_{pcb.Variant}_assemblyDrawing.pdf"), FileMode.Create)) {  
                await assemblyDrawing.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{pcb.Name}_{pcb.Variant}_3dModel.stl"), FileMode.Create)) {  
                await threeDModel.CopyToAsync(outputFileStream);  
            } 
            
            return pcb;
        }

        public async Task<List<Pcb>> GetPcbWithEntityAsync(string partNumber)
        {
            return await _db.Pcbs.GetProjectsWithEntityAsync(partNumber);
        }

        public async Task<List<Pcb>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord)) 
                return new List<Pcb>();
            return await _db.Pcbs.SearchByKeyWordAsync(keyWord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var project = await GetByIdAsync(id);
            await DeleteAsync(project);
        }

        public override async Task DeleteAsync(Pcb pcb)
        {
            await base.DeleteAsync(pcb);
            var path = Path.Combine(("wwwroot" + pcb.BOMFilePath).Split('/')[..^1]);
            if (Directory.Exists(path))  
            {  
                Directory.Delete(path, true);  
            } 
        }

        public override async Task<Pcb> GetByIdAsync(int id)
        {
            var result = (await _db.Pcbs.FindAsync(x => x.Id == id, "Entities")).FirstOrDefault();
            if (result == null)
                return null;
            
            foreach (var entity in result.Entities)
            {
                entity.Entity = await _entityExtService.GetByIdAsync(entity.EntityId);
            }

            result.Entities = result.Entities.OrderBy(x => x.Designator).ToList();
            return result;
        }

        public override async Task CreateAsync(Pcb item)
        {
            if (item.ReportDate == default)
            {
                item.ReportDate = DateTime.Now;
            }
            await base.CreateAsync(item);
        }

        public async Task IncreaseQuantityAsync(int id, int quantity)
        {
            await ChangeQuantityAsync(id, quantity);
        }
        
        public async Task DecreaseQuantityAsync(int id, int quantity)
        {
            await ChangeQuantityAsync(id, -quantity);
        }

        public async Task ChangeQuantityAsync(int id, int quantity)
        {
            if (quantity == 0)
            {
                return;
            }

            var entity = await _currentRepository.GetByIdAsync(id);
            entity.Quantity += quantity;
            if (entity.Quantity < 0)
                throw new NotImplementedException();
            await _currentRepository.UpdateAsync(entity);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = entity.Id, ElementType = ElementType.Design});
            await _db.SaveAsync();
        }
    }
}