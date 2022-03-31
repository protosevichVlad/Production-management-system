using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using MySqlConnector;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Data
{
    public interface IMySqlTableHelper
    {
        void CreateTable(DatabaseTable table);
        void DeleteTable(DatabaseTable table);
        void AddColumn(DatabaseTable table, TableColumn column);
        void DeleteColumn(DatabaseTable table, TableColumn column);
        List<BaseAltiumDbEntity> GetDataFromTable(DatabaseTable table);
        void InsertIntoTable(DatabaseTable table, BaseAltiumDbEntity data);
        void InsertListIntoTable(DatabaseTable table, List<BaseAltiumDbEntity> data);
        void UpdateDataInTable(DatabaseTable table, string partNumber, BaseAltiumDbEntity data);
        BaseAltiumDbEntity GetEntityByPartNumber(DatabaseTable table, string partNumber);
        void DeleteEntity(DatabaseTable table, string partNumber);
        void UpdateLibraryPropertyInTable(DatabaseTable table, string propertyName, string value);
        void RenameColumn(DatabaseTable table, TableColumn oldColumn, TableColumn newColumn);
        List<string> GetFiledFromAllTables(List<DatabaseTable> tables, string filed);
        List<string> GetFiledTable(DatabaseTable table, string filed);
    }
    
    public class MySqlTableHelper : IMySqlTableHelper
    {
        private MySqlConnection conn;

        public MySqlTableHelper(string connectionString)
        {
            this.conn = new MySqlConnection(connectionString);
        }
        
        public MySqlTableHelper(DbConnection connection)
        {
            this.conn = (MySqlConnection) connection;
        }

        public void CreateTable(DatabaseTable table)
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = table.GenerateCreateTableSqlQuery();
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

        public void DeleteTable(DatabaseTable table)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DROP TABLE `{table.TableName}`;";
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

        public void AddColumn(DatabaseTable table, TableColumn column)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `{table.TableName}` ADD `{column.ColumnName}` {GetTypeName(MySqlDbType.String)};";
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

        public void DeleteColumn(DatabaseTable table, TableColumn column)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `{table.TableName}` DROP COLUMN `{column.ColumnName}`;";
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
        public List<BaseAltiumDbEntity> GetDataFromTable(DatabaseTable table)
        {
            try
            {
                List<BaseAltiumDbEntity> result = new List<BaseAltiumDbEntity>();
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}`;";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var row = new BaseAltiumDbEntity();
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

        public void InsertIntoTable(DatabaseTable table, BaseAltiumDbEntity data)
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

        public void InsertListIntoTable(DatabaseTable table, List<BaseAltiumDbEntity> data)
        {
            throw new NotImplementedException();
        }

        public void UpdateDataInTable(DatabaseTable table, string partNumber,  BaseAltiumDbEntity data)
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

        public BaseAltiumDbEntity GetEntityByPartNumber(DatabaseTable table, string partNumber)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}` WHERE `Part Number`= @PartNumber;";
                cmd.Parameters.Add("@PartNumber", DbType.String).Value = partNumber;
                MySqlDataReader reader = cmd.ExecuteReader();
                var row = new BaseAltiumDbEntity();
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

        public void DeleteEntity(DatabaseTable table, string partNumber)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM `{table.TableName}` WHERE `Part Number` = @PartNumber;";
                cmd.Parameters.Add("@PartNumber", DbType.String).Value = partNumber;
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

        public void UpdateLibraryPropertyInTable(DatabaseTable table, string propertyName, string value)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"UPDATE `{table.TableName}` SET `{propertyName}` = @value";
                cmd.Parameters.Add("value", DbType.String).Value = value;
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

        public void RenameColumn(DatabaseTable table, TableColumn oldColumn, TableColumn newColumn)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"ALTER TABLE `{table.TableName}` RENAME COLUMN `{oldColumn.ColumnName}` TO `{newColumn.ColumnName}`;";
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

        public List<string> GetFiledFromAllTables(List<DatabaseTable> tables, string filed)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                StringBuilder command = new StringBuilder("");
                foreach (var table in tables)
                {
                    command.Append($"SELECT DISTINCT `{filed}` FROM `{table.TableName}` UNION DISTINCT ");
                }

                command.Remove(command.Length - 15, 15);
                cmd.CommandText = command.ToString();
                MySqlDataReader reader = cmd.ExecuteReader();
                List<string> result = new List<string>();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
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

        public List<string> GetFiledTable(DatabaseTable table, string filed)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT DISTINCT `{filed}` FROM `{table.TableName}`";
                MySqlDataReader reader = cmd.ExecuteReader();
                List<string> result = new List<string>();
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
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

        private string GetTypeName(MySqlDbType type)
        {
            return type switch
            {
                MySqlDbType.Bit => "BIT",
                MySqlDbType.Int32 => "INT",
                MySqlDbType.Double => "FLOAT",
                MySqlDbType.String => "VARCHAR(511)",
                _ => throw new NotImplementedException(),
            };
        }
    }
}