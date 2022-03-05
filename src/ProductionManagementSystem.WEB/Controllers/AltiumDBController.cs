﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
                ColumnName = "Id",
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
        [Route("[controller]/Table_{DatabaseTableName}")]
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
            return PartialView("Shared/Table/TableColumn", new TableColumn(){Id = index});
        }

        [HttpDelete]
        [Route("AltiumDB/Tables/{tableName}")]
        public async Task<IActionResult> Tables([FromRoute]string tableName)
        {
            await _databaseService.DeleteByTableNameAsync(tableName);
            return Ok();
        }
    }
}