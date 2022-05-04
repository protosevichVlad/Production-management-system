using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Models.PCB;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;
using Directory = System.IO.Directory;

namespace ProductionManagementSystem.Core.Services
{
    public interface IPcbService : IBaseService<Pcb>, ICalculableService
    {
        Task<Pcb> ImportPcbAsync(Stream bom, Stream image, Stream circuitDiagram,
            Stream assemblyDrawing, Stream threeDModel);

        Task UpdateAsync(CreateEditPcb createEditPcb);
        Task CreateAsync(CreateEditPcb createEditPcb);

        Task<List<Pcb>> GetPcbWithEntityAsync(string partNumber);
        Task<List<Pcb>> SearchByKeyWordAsync(string keyWord);
    }
    public class PcbService : BaseService<Pcb, IUnitOfWork>, IPcbService
    {
        private readonly IEntityExtService _entityExtService;
        private readonly IFileService _fileService;
        
        public PcbService(IUnitOfWork db, IEntityExtService entityService, IFileService fileService) : base(db)
        {
            _entityExtService = entityService;
            _fileService = fileService;
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
                
                pcb.UsedItems = new List<UsedItem>();
                int row = 10;
                while (true)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[row, 6].Text))
                        break;
                    
                    UsedItem entityInPcb = new UsedItem() { InItemType = CDBItemType.PCB, ItemType = CDBItemType.Entity};
                    entityInPcb.Designator = sheet.Cells[row, 4].Text;
                    entityInPcb.ItemId =
                        (await _entityExtService.GetEntityExtByPartNumber(sheet.Cells[row, 6].Text))?.KeyId  ?? 0;
                    entityInPcb.Quantity = int.Parse(sheet.Cells[row, 10].Text);

                    pcb.UsedItems.Add(entityInPcb);
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

        public async Task UpdateAsync(CreateEditPcb createEditPcb)
        {
            await UpdateAsync(await createEditPcb.GetPcb(_fileService));
        }

        public async Task CreateAsync(CreateEditPcb createEditPcb)
        {
            await CreateAsync(await createEditPcb.GetPcb(_fileService));
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
            var urls = new List<string>()
            {
                pcb.ImagePath,
                pcb.CircuitDiagramPath,
                pcb.AssemblyDrawingPath,
                pcb.ThreeDModelPath,
                pcb.BOMFilePath,
            };
            
            foreach (var url in urls)
            {
                await _fileService.DeleteFileByUrl(url);
            }
            
            await base.DeleteAsync(pcb);
        }

        public override async Task<Pcb> GetByIdAsync(int id)
        {
            var result = await _currentRepository.GetByIdAsync(id);
            result.UsedItems = result.UsedItems.OrderBy(x => x.Designator).ToList();
            return result;
        }

        public override async Task CreateAsync(Pcb item)
        {
            if (item.ReportDate == default)
            {
                item.ReportDate = DateTime.Now;
            }
            await base.CreateAsync(item);
            item.UsedItems.ForEach(x => x.InItemId = item.Id);
            await _db.UsedItemRepository.UpdateRangeAsync(item.UsedItems);
            await _db.SaveAsync();
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
            if (entity == null) return;
            entity.Quantity += quantity;
            if (entity.Quantity < 0)
                throw new NotImplementedException();
            await _db.Pcbs.UpdateQuantityAsync(entity.Id, entity.Quantity);

            await _db.ElementDifferenceRepository.CreateAsync(new ElementDifference()
                {Difference = quantity, ElementId = entity.Id, ElementType = ElementType.Design});
            await _db.SaveAsync();
        }

        public override async Task UpdateAsync(Pcb item)
        {
            var pcbFromDb = await GetByIdAsync(item.Id);
            item.ImagePath = await _fileService.GetNewUrl(item.ImagePath, pcbFromDb.ImagePath);
            item.AssemblyDrawingPath = await _fileService.GetNewUrl(item.AssemblyDrawingPath, pcbFromDb.AssemblyDrawingPath);
            item.CircuitDiagramPath = await _fileService.GetNewUrl(item.CircuitDiagramPath, pcbFromDb.CircuitDiagramPath);
            item.ThreeDModelPath = await _fileService.GetNewUrl(item.ThreeDModelPath, pcbFromDb.ThreeDModelPath);
            item.BOMFilePath = await _fileService.GetNewUrl(item.BOMFilePath, pcbFromDb.BOMFilePath);
            
            await base.UpdateAsync(item);
        }
    }
}