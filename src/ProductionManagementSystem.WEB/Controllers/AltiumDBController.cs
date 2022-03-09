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

namespace ProductionManagementSystem.WEB.Controllers
{
    public class AltiumDBController : Controller
    {
        private IDatabaseService _databaseService;

        public AltiumDBController(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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
            table.TableColumns = new List<TableColumn>();
            table.TableColumns.Add(new TableColumn()
            {
                Display = false,
                ColumnName = "KeyID",
                ColumnType = MySqlDbType.Int32,
            });
            return View(table);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable(DatabaseTable table)
        {
            await _databaseService.CreateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {DatabaseTableName = table.TableName});
        }

        [HttpGet]
        [Route("[controller]/Tables/{DatabaseTableName}")]
        public async Task<IActionResult> GetDataFromTable(string DatabaseTableName)
        {
            DataListViewModel vm = new DataListViewModel()
            {
                DatabaseTable = await _databaseService.GetTableByNameAsync(DatabaseTableName),
                Data = await _databaseService.GetDataFromTableAsync(DatabaseTableName)
            };
            return View("GetDataFromTable", vm);
        }
        
        [HttpGet]
        [Route("[controller]/EditTable/{DatabaseTableName}")]
        public async Task<IActionResult> EditTable(string DatabaseTableName)
        {
            var table = await _databaseService.GetTableByNameAsync(DatabaseTableName);
            return View("CreateTable", table);
        }
        
        [HttpPost]
        [Route("[controller]/EditTable/{DatabaseTableName}")]
        public async Task<IActionResult> EditTable(DatabaseTable table)
        {
            await _databaseService.UpdateAsync(table);
            return RedirectToAction(nameof(GetDataFromTable), new {DatabaseTableName = table.TableName});
        }
        
        public async Task<IActionResult> GetPartialViewForTableColumn(int index)
        {
            return PartialView("Shared/Table/TableColumn", new TableColumn()
            {
                Id = index, 
                ColumnType = MySqlDbType.String,
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
            return View("CreateEntity",new EntityViewModel() {Table = table});
        }
        
        [HttpPost]
        [Route("AltiumDB/Tables/{tableName}/CreateEntity")]
        public async Task<IActionResult> CreateEntity([FromRoute]string tableName, Dictionary<string, string> data)
        {
            await _databaseService.InsertIntoTableByTableNameAsync(tableName,
                data.ToDictionary(pair => pair.Key, pair=>(object)pair.Value));
            return RedirectToAction(nameof(GetDataFromTable), new {DatabaseTableName = tableName});
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{id:int}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]int id)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityById(tableName, id);
            return View("CreateEntity",new EntityViewModel() {Table = table, Data = data});
        }
        
        [HttpPost]
        [Route("AltiumDB/Tables/{tableName}/EditEntity/{id:int}")]
        public async Task<IActionResult> EditEntity([FromRoute]string tableName, [FromRoute]int id, Dictionary<string, string> data)
        {
            await _databaseService.UpdateEntityAsync(tableName, id,
                data.ToDictionary(pair => pair.Key, pair=>(object)pair.Value));
            return RedirectToAction(nameof(GetDataFromTable), new {DatabaseTableName = tableName});
        }
        
        [HttpDelete]
        [Route("AltiumDB/Tables/{tableName}/{id:int}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName, [FromRoute]int id)
        {
            await _databaseService.DeleteEntityById(tableName, id);
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
                    return RedirectToAction(nameof(GetDataFromTable), new {DatabaseTableName = "AltiumDB_" + tableName});
                case "xlsx":
                    await _databaseService.ImportFromFile(tableName, new StreamReader(file.OpenReadStream()), new ExcelImporter());
                    break;
                
                default:
                    throw new NotImplementedException();
            }
            
            return RedirectToAction(nameof(Tables));
        }
        
        [HttpGet]
        [Route("AltiumDB/Tables/{tableName}/{id:int}")]
        public async Task<IActionResult> EntityDetails([FromRoute]string tableName, [FromRoute]int id)
        {
            var table = await _databaseService.GetTableByNameAsync(tableName);
            var data = await _databaseService.GetEntityById(tableName, id);
            return View(new EntityViewModel()
            {
                Table = table,
                Data = data
            });
        }
    }
}