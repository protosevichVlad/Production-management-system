﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Services.AltiumDB;
using ProductionManagementSystem.WEB.Models.AltiumDB;
using ProductionManagementSystem.WEB.Models.Components;

namespace ProductionManagementSystem.WEB.Controllers
{
    public class AltiumDBController : Controller
    {
        private IDatabaseService _databaseService;
        private IDirectoryService _directoryService;
        
        public AltiumDBController(IDatabaseService databaseService, IDirectoryService directoryService)
        {
            _databaseService = databaseService;
            _directoryService = directoryService;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<ViewResult> Tables()
        {
            return View(await _databaseService.GetAllAsync());
        }

        public async Task<ViewResult> CreateTable()
        {
            var table = new DatabaseTable();
            table.InitAltiumDB("");
            ViewBag.DirectoriesTree = new TreeViewViewModel(await _directoryService.GetByIdAsync(0), false) { ShowPath = true};
            return View(table);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable(DatabaseTable table)
        {
            await _databaseService.CreateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = table.TableName});
        }

        [HttpGet]
        [Route("[controller]/Tables/{tableName}")]
        public async Task<IActionResult> GetDataFromTable(string tableName)
        {
            DataListViewModel vm = new DataListViewModel()
            {
                DatabaseTable = await _databaseService.GetTableByNameAsync(tableName),
                Data = await _databaseService.GetDataFromTableAsync(tableName)
            };
            return View("GetDataFromTable", vm);
        }
        
        [HttpGet]
        [Route("[controller]/EditTable/{tableName}")]
        public async Task<IActionResult> EditTable(string tableName)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            ViewBag.DirectoriesTree = new TreeViewViewModel(await _directoryService.GetByIdAsync(0), false)
            {
                ShowPath = true,
                SelectedId = table.DirectoryId ?? 0,
            };
            return View("CreateTable", table);
        }
        
        [HttpPost]
        [Route("[controller]/EditTable/{tableName}")]
        public async Task<IActionResult> EditTable(DatabaseTable table)
        {
            await _databaseService.UpdateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = table.TableName});
        }
        
        public async Task<IActionResult> GetPartialViewForTableColumn(int index)
        {
            return PartialView("Shared/Table/TableColumn", new TableColumn()
            {
                DatabaseOrder = index,
            });
        }

        [HttpDelete]
        [Route("AltiumDB/Tables/{tableName}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName)
        {
            await _databaseService.DeleteByTableNameAsync(tableName);
            return Ok();
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromRoute]string tableName)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            ViewBag.Manufacturers = await _databaseService.GetFiledFromAllTables("Manufacturer");
            ViewBag.Categories = await _databaseService.GetFiledTable(tableName, "Category");
            ViewBag.Suppliers = await _databaseService.GetFiledFromAllTables("Supplier");
            ViewBag.Cases = await _databaseService.GetFiledFromAllTables("Case");
            ViewBag.LibraryRefs = await _databaseService.GetFiledTable(tableName, "Library Ref");
            ViewBag.FootprintRefs = await _databaseService.GetFiledTable(tableName, "Footprint Ref");
            return View("CreateEntity", new BaseAltiumDbEntity(table));
        }
        
        [HttpPost]
        [Route("AltiumDB/Tables/{tableName}/CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromRoute]string tableName, BaseAltiumDbEntity data)
        {
            await _databaseService.InsertIntoTableByTableNameAsync(tableName, data);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = tableName});
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{partNumber}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityByPartNumber(tableName, partNumber);
            ViewBag.Manufacturers = await _databaseService.GetFiledFromAllTables("Manufacturer");
            ViewBag.Categories = await _databaseService.GetFiledTable(tableName, "Category");
            ViewBag.Suppliers = await _databaseService.GetFiledFromAllTables("Supplier");
            ViewBag.Cases = await _databaseService.GetFiledFromAllTables("Case");
            ViewBag.LibraryRefs = await _databaseService.GetFiledTable(tableName, "Library Ref");
            ViewBag.FootprintRefs = await _databaseService.GetFiledTable(tableName, "Footprint Ref");
            return View("CreateEntity", data);
        }
        
        [HttpPost]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{partNumber}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]string partNumber, BaseAltiumDbEntity data)
        {
            await _databaseService.UpdateEntityAsync(tableName, partNumber,data);
            return RedirectToAction(nameof(GetDataFromTable), new {tableName = tableName});
        }
        
        [HttpDelete]
        [Route("AltiumDB/Tables/{tableName}/{partNumber}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            await _databaseService.DeleteEntityById(tableName, partNumber);
            return Ok();
        }

        [HttpGet]
        [Route("AltiumDB/ImportTables")]
        public async Task<IActionResult> ImportTables()
        {
            return View();
        }
        
        [HttpPost]
        [Route("AltiumDB/ImportTables")]
        public async Task<IActionResult> ImportTables(IFormFile file)
        {
            var tableName = file.FileName.Split('.')[0];
            var fileExtension = file.FileName.Split('.')[^1];
            switch (fileExtension)
            {
                case "csv":
                    await _databaseService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new CsvDataImporter());
                    return RedirectToAction(nameof(GetDataFromTable), new {tableName = "AltiumDB " + tableName});
                case "xlsx":
                case "xls":
                    await _databaseService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new ExcelImporter());
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            return RedirectToAction(nameof(Tables));
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/{partNumber}")]
        public async Task<IActionResult> EntityDetails([FromRoute]string tableName, [FromRoute]string partNumber)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityByPartNumber(tableName, partNumber);
            return View(data);
        }
        
        [HttpGet]
        [Route("AltiumDB/Directories")]
        public async Task<IActionResult> Directories()
        {
            return View(new TreeViewViewModel(await _directoryService.GetByIdAsync(0), true){ ShowPath = true});
        }
    }
}