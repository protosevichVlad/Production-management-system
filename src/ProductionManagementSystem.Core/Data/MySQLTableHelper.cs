using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using MySqlConnector;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Data
{
    public interface IMySQLTableHelper
    {
        void CreateTable(DatabaseTable table);
        void DeleteTable(DatabaseTable table);
        void AddColumn(DatabaseTable table, TableColumn column);
        void DeleteColumn(DatabaseTable table, TableColumn column);
        List<Dictionary<string, object>> GetDataFromTable(DatabaseTable table);
        void InsertIntoTable(DatabaseTable table, IDictionary<string, object> data);
        void InsertListIntoTable(DatabaseTable table, List<IDictionary<string, object>> data);
        void UpdateDataInTable(DatabaseTable table, int id, IDictionary<string, object> data);
        Dictionary<string, object> GetEntityById(DatabaseTable table, int id);
        void DeleteEntity(DatabaseTable table, int id);
    }
    
    public class MySQLTableHelper : IMySQLTableHelper
    {
        private MySqlConnection conn;

        public MySQLTableHelper(string connectionString)
        {
            this.conn = new MySqlConnection(connectionString);
        }

        public void CreateTable(DatabaseTable table)
        {
            try
            {
                conn.Open();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"CREATE TABLE `{table.TableName}` (Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY";
                for (int i = 0; i < table.TableColumns.Count; i++)
                {
                    if (table.TableColumns[i].ColumnName == "Id")
                        continue;
                    
                    cmd.CommandText += $", `{table.TableColumns[i].ColumnName}` {GetTypeName(table.TableColumns[i].ColumnType)}";
                }
                
                cmd.CommandText += $");";
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
                cmd.CommandText = $"ALTER TABLE `{table.TableName}` ADD `{column.ColumnName}` {GetTypeName(column.ColumnType)};";
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
        public List<Dictionary<string, object>> GetDataFromTable(DatabaseTable table)
        {
            try
            {
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}`;";
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(table.TableColumns[i].ColumnName, reader[i]);
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

        public void InsertIntoTable(DatabaseTable table, IDictionary<string, object> data)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO `{table.TableName}` ({table.GetColumns()}) VALUES ({table.GenerateValueBinding()})";
                foreach (var column in table.TableColumns.Where(x => x.ColumnName != "Id"))
                {
                    cmd.Parameters.Add(column.ParameterName, column.ColumnType).Value = data[column.ColumnName];
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

        public void InsertListIntoTable(DatabaseTable table, List<IDictionary<string, object>> data)
        {
            throw new NotImplementedException();
        }

        public void UpdateDataInTable(DatabaseTable table, int id,  IDictionary<string, object> data)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText =
                    $"UPDATE `{table.TableName}` SET {table.GenerateUpdateBinding()} WHERE Id = @Id";
                cmd.Parameters.Add("@Id", DbType.Int32).Value = id;
                foreach (var column in table.TableColumns.Where(x => x.ColumnName != "Id"))
                {
                    cmd.Parameters.Add(column.ParameterName, column.ColumnType).Value = data[column.ColumnName];
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

        public Dictionary<string, object> GetEntityById(DatabaseTable table, int id)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT * FROM `{table.TableName}` WHERE Id= @Id;";
                cmd.Parameters.Add("@Id", DbType.Int32).Value = id;
                MySqlDataReader reader = cmd.ExecuteReader();
                var row = new Dictionary<string, object>();
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(table.TableColumns[i].ColumnName, reader[i]);
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

        public void DeleteEntity(DatabaseTable table, int id)
        {
            try
            {
                conn.Open();
                
                MySqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"DELETE FROM `{table.TableName}` WHERE Id= @Id;";
                cmd.Parameters.Add("@Id", DbType.Int32).Value = id;
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

        private string GetTypeName(MySqlDbType type)
        {
            return type switch
            {
                MySqlDbType.Bit => "BIT",
                MySqlDbType.Int32 => "INT",
                MySqlDbType.Double => "FLOAT",
                MySqlDbType.String => "VARCHAR(255)",
                _ => throw new NotImplementedException(),
            };
        }
    }
}