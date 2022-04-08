using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Models.AltiumDB.Projects;
using ProductionManagementSystem.Core.Models.ElementsDifference;
using ProductionManagementSystem.Core.Repositories;
using ProductionManagementSystem.Core.Repositories.AltiumDB;
using Directory = System.IO.Directory;

namespace ProductionManagementSystem.Core.Services.AltiumDB
{
    public interface IProjectService : IBaseService<Project>
    {
        Task<Project> ImportProjectAsync(Stream bom, Stream image, Stream circuitDiagram,
            Stream assemblyDrawing, Stream threeDModel);

        Task<List<Project>> GetProjectsWithEntityAsync(string partNumber);
        Task<List<Project>> SearchByKeyWordAsync(string keyWord);
        Task DeleteByIdAsync(int id);
        Task IncreaseQuantityAsync(int id, int quantity);
        Task DecreaseQuantityAsync(int id, int quantity);
        Task ChangeQuantityAsync(int id, int quantity);
    }
    public class ProjectService : BaseService<Project, IUnitOfWork>, IProjectService
    {
        private readonly IEntityExtService _entityExtService;
        
        public ProjectService(IUnitOfWork db, IEntityExtService entityService) : base(db)
        {
            _entityExtService = entityService;
            _currentRepository = _db.Projects;
        }

        public async Task<Project> ImportProjectAsync(Stream bom, Stream image, Stream circuitDiagram,
            Stream assemblyDrawing, Stream threeDModel)
        {
            Project project = new Project();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Stream bomExcel = new MemoryStream();
            await bom.CopyToAsync(bomExcel);
            using(var package = new ExcelPackage(bomExcel))
            {
                var sheet = package.Workbook.Worksheets[0];
                project.Name = sheet.Cells["D4"].Text;
                if (project.Name.EndsWith(".PrjPcb"))
                {
                    project.Name = project.Name.Substring(0, project.Name.Length - ".PrjPcb".Length);
                }
                
                project.Variant = sheet.Cells["D5"].Text;

                if (DateTime.TryParseExact(sheet.Cells["D7"].Text + "_" + sheet.Cells["E7"].Text, "dd.MM.yyyy_hh:mm",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var projectReportDate))
                    project.ReportDate = projectReportDate;
                else
                    project.ReportDate = DateTime.Now;
                
                project.Entities = new List<EntityInProject>();
                int row = 10;
                while (true)
                {
                    if (string.IsNullOrWhiteSpace(sheet.Cells[row, 6].Text))
                        break;
                    
                    EntityInProject entityInProject = new EntityInProject();
                    entityInProject.Designator = sheet.Cells[row, 4].Text;
                    entityInProject.EntityId =
                        (await _entityExtService.GetEntityExtByPartNumber(sheet.Cells[row, 6].Text))?.KeyId  ?? 0;
                    entityInProject.Quantity = int.Parse(sheet.Cells[row, 10].Text);

                    project.Entities.Add(entityInProject);
                    row++;
                }
            }
            
            string basePath = Path.Combine("wwwroot", "uploads", project.Name, project.Variant);
            string wwwPath = Path.Combine("uploads", project.Name, project.Variant);
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }

            project.BOMFilePath = '/' + Path.Combine(wwwPath, $"{project.Name}_{project.Variant}_bom.xlsx").Replace('\\', '/');
            project.ImagePath = '/' + Path.Combine(wwwPath, $"{project.Name}_{project.Variant}_image.png").Replace('\\', '/');
            project.CircuitDiagramPath = '/' + Path.Combine(wwwPath, $"{project.Name}_{project.Variant}_circuitDiagram.pdf").Replace('\\', '/');
            project.AssemblyDrawingPath = '/' + Path.Combine(wwwPath, $"{project.Name}_{project.Variant}_assemblyDrawing.pdf").Replace('\\', '/');
            project.ThreeDModelPath = '/' + Path.Combine(wwwPath, $"{project.Name}_{project.Variant}_3dModel.stl").Replace('\\', '/');
            project.Description = "This project was created with using a BOM file";
            project.Quantity = 0;
            
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{project.Name}_{project.Variant}_bom.xlsx"), FileMode.Create)) {  
                await bom.CopyToAsync(outputFileStream);  
            }
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{project.Name}_{project.Variant}_image.png"), FileMode.Create)) {  
                await image.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{project.Name}_{project.Variant}_circuitDiagram.pdf"), FileMode.Create)) {  
                await circuitDiagram.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{project.Name}_{project.Variant}_assemblyDrawing.pdf"), FileMode.Create)) {  
                await assemblyDrawing.CopyToAsync(outputFileStream);  
            } 
            await using(FileStream outputFileStream = new FileStream(Path.Combine(basePath ,$"{project.Name}_{project.Variant}_3dModel.stl"), FileMode.Create)) {  
                await threeDModel.CopyToAsync(outputFileStream);  
            } 
            
            return project;
        }

        public async Task<List<Project>> GetProjectsWithEntityAsync(string partNumber)
        {
            return await _db.Projects.GetProjectsWithEntityAsync(partNumber);
        }

        public async Task<List<Project>> SearchByKeyWordAsync(string keyWord)
        {
            if (string.IsNullOrEmpty(keyWord)) 
                return new List<Project>();
            return await _db.Projects.SearchByKeyWordAsync(keyWord);
        }

        public async Task DeleteByIdAsync(int id)
        {
            var project = await GetByIdAsync(id);
            await DeleteAsync(project);
        }

        public override async Task DeleteAsync(Project project)
        {
            var path = Path.Combine(("wwwroot" + project.BOMFilePath).Split('/')[..^1]);
            if (Directory.Exists(path))  
            {  
                Directory.Delete(path, true);  
            } 
            
            await base.DeleteAsync(project);
        }

        public override async Task<Project> GetByIdAsync(int id)
        {
            var result = (await _db.Projects.FindAsync(x => x.Id == id, "Entities")).FirstOrDefault();
            if (result == null)
                return null;
            
            foreach (var entity in result.Entities)
            {
                entity.Entity = await _entityExtService.GetByIdAsync(entity.EntityId);
            }

            result.Entities = result.Entities.OrderBy(x => x.Designator).ToList();
            return result;
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