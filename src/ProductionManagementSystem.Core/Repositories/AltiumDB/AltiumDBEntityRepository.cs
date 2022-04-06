using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using ProductionManagementSystem.Core.Data;
using ProductionManagementSystem.Core.Data.EF;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Repositories.AltiumDB
{
    public interface IAltiumDBEntityRepository : IRepository<AltiumDbEntity>
    {
        List<AltiumDbEntity> GetAllFromTable(Table table);
        Task CreateAsync(Table table, AltiumDbEntity data);
        Task UpdateAsync(Table table, string partNumber, AltiumDbEntity data);
        Task<AltiumDbEntity> GetByPartNumber(Table table, string partNumber);
        Task<AltiumDbEntity> GetByPartNumber(string partNumber);

        Task<List<AltiumDbEntity>> SearchByKeyWordAsync(Table table, string keyWord);
        Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord);
    }

    public class AltiumDBEntityRepository : IAltiumDBEntityRepository
    {
        private MySqlConnection conn;
        private ApplicationContext _context;

        public AltiumDBEntityRepository(ApplicationContext context)
        {
            _context = context;
            conn = (MySqlConnection) context.Database.GetDbConnection();
        }
        
        public async Task<List<AltiumDbEntity>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<AltiumDbEntity> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AltiumDbEntity>> FindAsync(Func<AltiumDbEntity, bool> predicate, string includeProperty = null)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(AltiumDbEntity item)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Table table, AltiumDbEntity data)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO `{table.TableName}` ({table.GetColumns()}) VALUES ({table.GenerateValueBinding()})";
                foreach (var column in table.TableColumns)
                {
                    cmd.Parameters.Add(column.ParameterName, MySqlDbType.String).Value = data[column.ColumnName];
                }

                cmd.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                conn.Close(); 
            }
        }

        public async Task UpdateAsync(Table table, string partNumber, AltiumDbEntity data)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"UPDATE `{table.TableName}` SET {table.GenerateUpdateBinding()} WHERE `Part Number` = @WherePartNumber";
                cmd.Parameters.Add("@WherePartNumber", DbType.String).Value = partNumber;
                foreach (var column in table.TableColumns)
                {
                    cmd.Parameters.Add(column.ParameterName, MySqlDbType.String).Value = data[column.ColumnName];
                }

                cmd.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                conn.Close(); 
            }
        }

        public async Task<AltiumDbEntity> GetByPartNumber(Table table, string partNumber)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}` WHERE `Part Number`= @PartNumber;";
                cmd.Parameters.Add("@PartNumber", DbType.String).Value = partNumber;
                MySqlDataReader reader = cmd.ExecuteReader();
                var row = new AltiumDbEntity();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[table.TableColumns[i].ColumnName] = reader[i].ToString();
                    }
                }
                
                reader.Close();
                return row;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                conn.Close(); 
            }
        }

        public async Task<AltiumDbEntity> GetByPartNumber(string partNumber)
        {
            AltiumDbEntity result = null;
            var tables = _context.Tables.Include(x => x.TableColumns).ToList();
            foreach (var table in tables)
            {
                table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
                result = await GetByPartNumber(table, partNumber);
                if (result != null && !string.IsNullOrWhiteSpace(result.PartNumber))
                {
                    result["tableName"] = table.TableName;
                    break;
                }
            }
            
            return result;
        }

        public async Task<List<AltiumDbEntity>> SearchByKeyWordAsync(Table table, string keyWord)
        {
            try
            {
                var result = new List<AltiumDbEntity>();
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}` WHERE {string.Join(" OR ", table.TableColumns.Select(x => $"`{x.ColumnName}` LIKE @KeyWordExpression"))}";
                cmd.Parameters.Add("@KeyWordExpression", DbType.String).Value = "%" + keyWord + "%";
                MySqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var row = new AltiumDbEntity();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[table.TableColumns[i].ColumnName] = reader[i].ToString();
                    }

                    row["tableName"] = table.TableName;
                    result.Add(row);
                }
                
                reader.Close();
                return result;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                await conn.CloseAsync(); 
            }
        }

        public async Task<List<AltiumDbEntity>> SearchByKeyWordAsync(string keyWord)
        {
            List<AltiumDbEntity> result = new List<AltiumDbEntity>();
            var tables = _context.Tables.Include(x => x.TableColumns).ToList();
            foreach (var table in tables)
            {
                table.TableColumns = table.TableColumns.OrderBy(x => x.DatabaseOrder).ToList();
                result.AddRange(await SearchByKeyWordAsync(table, keyWord));
            }
            
            return result;
        }

        public async Task UpdateAsync(AltiumDbEntity item)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateRangeAsync(List<AltiumDbEntity> items)
        {
            throw new NotImplementedException();
        }

        public void Delete(AltiumDbEntity item)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public List<AltiumDbEntity> GetAllFromTable(Table table)
        {
            try
            {
                List<AltiumDbEntity> result = new List<AltiumDbEntity>();
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}`;";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var row = new AltiumDbEntity();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[table.TableColumns[i].ColumnName] = reader[i].ToString();
                    }
                    
                    result.Add(row);
                }
                
                reader.Close();
                return result;
            }
            catch(MySqlException ex)
            {
                throw ex;
            }
            finally
            {  
                conn.Close(); 
            }
        }
    }
}