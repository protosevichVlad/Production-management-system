using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Exceptions;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;
using ProductionManagementSystem.Core.Repositories.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories
{
    public interface IEntityExtRepository : IRepository<EntityExt>
    {
        Task<List<EntityExt>> GetAllByTableId(int id);
        Task<EntityExt> GetByPartNumber(string partNumber, int? tableId=null);
        Task<List<string>> GetValues(string column, string tableName);
        Task<List<string>> GetValues(string column, int? tableId=null);
        Task<List<EntityExt>> SearchByKeyWordAsync(string s, int? tableId=null);
        Task UpdateQuantity(int id, int newQuantity);
    }

    public class EntityExtRepository : IEntityExtRepository
    {
        private readonly DbConnection _conn;
        private readonly ApplicationContext _context;

        public EntityExtRepository(ApplicationContext context)
        {
            _context = context;
            _conn = context.Database.GetDbConnection();
        }
        
        public async Task<List<EntityExt>> GetAllAsync()
        {
            var tables = await _context.Tables.Include(x => x.TableColumns).ToListAsync();
            if (tables.Count == 0) return new List<EntityExt>();
            try
            {
                await _conn.OpenAsync();
                
                var cmd = _conn.CreateCommand();
                cmd.CommandText = string.Join(" UNION ", tables.Select((x) => GetSqlSelectAllColumnsEntityExt(x)));
                return await GetListEntityExt(cmd);
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        private async Task<List<EntityExt>> GetListEntityExt(DbCommand cmd, Table table=null)
        {
            List<EntityExt> result = new List<EntityExt>();
            var reader = await cmd.ExecuteReaderAsync();
            var fields = table?.TableColumns.Select(x => x.ColumnName) ?? AltiumDbEntity.Fields;
            while (await reader.ReadAsync())
            {
                var row = new EntityExt();
                foreach (var column in fields)
                {
                    row[column] = reader[column] as string;
                }

                row.Quantity = (int) reader["Quantity"];
                row.TableId = (int) reader["TableId"];
                row.KeyId = (int) reader["KeyId"];
                row.ImageUrl = reader["ImageUrl"] as string;
                result.Add(row);
            }
                
            await reader.CloseAsync();
            return result;
        }

        public async Task<EntityExt> GetByIdAsync(int id)
        {
            var tables = await _context.Tables.Include(x => x.TableColumns).ToListAsync();
            try
            {
                await _conn.OpenAsync();
                var cmd = _conn.CreateCommand();
                cmd.CommandText = string.Join(" UNION ",
                    tables.Select(table => $"{GetSqlSelectAllColumnsEntityExt(table)} WHERE `KeyId`= '{id}'"));
                return (await GetListEntityExt(cmd)).FirstOrDefault();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task<List<EntityExt>> FindAsync(Func<EntityExt, bool> predicate, string includeProperty = null)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(EntityExt item)
        {
            var altiumDbEntity = item.GetAltiumDbEntity();
            var entity = item.GetEntity();
            var table = await _context.Tables.Include(x => x.TableColumns).FirstOrDefaultAsync(x=> x.Id == entity.TableId);
            altiumDbEntity.LibraryPath = table.FootprintPath;
            altiumDbEntity.FootprintPath = table.FootprintPath;
            try
            {
                await _conn.OpenAsync();
                var cmd = _conn.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO `{table.TableName}` ({table.GetColumns()}) VALUES ({table.GenerateValueBinding()})";
                foreach (var column in table.TableColumns)
                {
                    AddParameterToCmd(cmd, column.ParameterName, altiumDbEntity[column.ColumnName]);
                }

                await cmd.ExecuteNonQueryAsync();

                await _context.Entities.AddAsync(entity);
                await _context.SaveChangesAsync();
                item.KeyId = entity.KeyId;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task UpdateAsync(EntityExt item)
        {
            var altiumDbEntity = item.GetAltiumDbEntity();
            var entity = item.GetEntity();
            var table = await _context.Tables.Include(x => x.TableColumns).FirstOrDefaultAsync(x=> x.Id == entity.TableId);
            try
            {
                await _conn.OpenAsync();
                var cmd = _conn.CreateCommand();
                cmd.CommandText =
                    $"UPDATE `{table.TableName}` SET {table.GenerateUpdateBinding()} WHERE `Part Number` = @WherePartNumber";
                AddParameterToCmd(cmd, "@WherePartNumber", altiumDbEntity.PartNumber);
                foreach (var column in table.TableColumns)
                {
                    AddParameterToCmd(cmd, column.ParameterName, altiumDbEntity[column.ColumnName]);
                }

                await cmd.ExecuteNonQueryAsync();
                
                _context.Entities.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task UpdateRangeAsync(List<EntityExt> items)
        {
            throw new NotImplementedException();
        }

        public void Delete(EntityExt item)
        {
            var used = _context.UsedItems.Any(x => x.ItemType == CDBItemType.Entity && x.ItemId == item.KeyId);
            if (used)
            {
                throw new DeleteReferenceException("Entity delete not permitted", "This entity used in device or pcb");
            }

            var table = _context.Tables.Find(item.TableId);
            try
            {
                _conn.Open();
                var cmd = _conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM `{table.TableName}` WHERE `Part Number` = @PartNumber;";
                cmd.CommandText += $"DELETE FROM `Entities` WHERE `PartNumber` = @PartNumber;";
                AddParameterToCmd(cmd, "@PartNumber", item.PartNumber);
                cmd.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                _conn.Close(); 
            }
        }

        public void DeleteRange(List<EntityExt> items)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<EntityExt> GetLatestAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<EntityExt>> GetAllByTableId(int id)
        {
            var table = await _context.Tables.Include(x => x.TableColumns).FirstOrDefaultAsync(x => x.Id == id);
            return await GetAllByTableAsync(table);
        }

        public async Task<List<string>> GetValues(string column, int? tableId=null)
        {
            var tables = await _context.Tables.Where(x => !tableId.HasValue || x.Id == tableId).ToListAsync();
            if (tables.Count == 0) return new List<string>();
            try
            {
                await _conn.OpenAsync();
                
                var cmd = _conn.CreateCommand();
                cmd.CommandText = string.Join(" UNION DISTINCT ",
                    tables.Select(table => $"SELECT DISTINCT `{column}` FROM `{table.TableName}` WHERE `{column}` IS NOT NUll AND `{column}` <> ''"));
                var reader = await cmd.ExecuteReaderAsync();
                List<string> result = new List<string>();
                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull(0))
                        result.Add( reader.GetString(0));
                }
                
                await reader.CloseAsync();
                return result;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task<List<EntityExt>> SearchByKeyWordAsync(string s, int? tableId = null)
        {
            var tables = await _context.Tables.Include(x => x.TableColumns).Where(x => !tableId.HasValue || x.Id == tableId).ToListAsync();
            if (tables.Count == 0) return new List<EntityExt>();
            try
            {
                await _conn.OpenAsync();
                
                var cmd = _conn.CreateCommand();
                cmd.CommandText = string.Join(" UNION ",
                    tables.Select(table => $"{GetSqlSelectAllColumnsEntityExt(table, tableId.HasValue)} WHERE {string.Join(" OR ", table.TableColumns.Select(x => $"`{x.ColumnName}` LIKE '%{s}%'"))}"));
                return await GetListEntityExt(cmd, tableId.HasValue ? tables[0]:null);
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task UpdateQuantity(int id, int newQuantity)
        {
            var entity = await _context.Entities.FindAsync(id);
            entity.Quantity = newQuantity;
        }

        public async Task<List<string>> GetValues(string column, string tableName)
        {
            var table = await _context.Tables.Include(x => x.TableColumns).FirstOrDefaultAsync(x => x.TableName == tableName);
            try
            {
                await _conn.OpenAsync();
                
                var cmd = _conn.CreateCommand();
                cmd.CommandText = $"SELECT DISTINCT `{column}` FROM `{table.TableName}`";
                var reader = await cmd.ExecuteReaderAsync();
                List<string> result = new List<string>();
                while (await reader.ReadAsync())
                {
                    if (!reader.IsDBNull(0))
                        result.Add( reader.GetString(0));
                }
                
                await reader.CloseAsync();
                return result;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        public async Task<EntityExt> GetByPartNumber(string partNumber, int? tableId=null)
        {
            var tables = await _context.Tables.Include(x => x.TableColumns).Where(x => !tableId.HasValue || x.Id == tableId).ToListAsync();
            if (tables.Count == 0) return null;
            try
            {
                await _conn.OpenAsync();
                var cmd = _conn.CreateCommand();
                cmd.CommandText = string.Join(" UNION ",
                    tables.Select(table => $"{GetSqlSelectAllColumnsEntityExt(table, tableId.HasValue)} WHERE `Part Number`= '{partNumber}'"));
                return (await GetListEntityExt(cmd, tableId.HasValue ? tables[0]:null)).FirstOrDefault();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        private async Task<List<EntityExt>> GetAllByTableAsync(Table table)
        {
            try
            {
                await _conn.OpenAsync();
                var cmd = _conn.CreateCommand();
                cmd.CommandText = GetSqlSelectAllColumnsEntityExt(table, true);
                return await GetListEntityExt(cmd, table);
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await _conn.CloseAsync(); 
            }
        }

        private string GetSqlSelectAllColumnsEntityExt(Table table, bool full=false)
        {
            table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
            var fields = full ? table?.TableColumns?.Select(x => x.ColumnName) : AltiumDbEntity.Fields;
            return
                $"SELECT {string.Join(", ", fields.Select(x => $"t.`{x}` as `{x}`"))}, " +
                $"e.Quantity, e.ImageUrl, e.KeyId, e.TableId " +
                $"FROM `{table.TableName}` as t " +
                $"INNER JOIN Entities as e ON e.PartNumber = t.`Part Number` AND e.TableId = {table.Id}";
        }

        private void AddParameterToCmd(DbCommand cmd, string parameterName, object value)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

    }
}