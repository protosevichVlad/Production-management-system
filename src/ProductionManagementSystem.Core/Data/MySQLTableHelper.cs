using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Text;
using MySqlConnector;
using ProductionManagementSystem.Core.Models;
using ProductionManagementSystem.Core.Models.AltiumDB;

namespace ProductionManagementSystem.Core.Data
{
    public interface IMySqlTableHelper
    {
        void CreateTable(Table table);
        void DeleteTable(Table table);
        void AddColumn(Table table, TableColumn column);
        void DeleteColumn(Table table, TableColumn column);
        void UpdateLibraryPropertyInTable(Table table, string propertyName, string value);
        void RenameColumn(Table table, TableColumn oldColumn, TableColumn newColumn);
        List<string> GetFiledFromAllTables(List<Table> tables, string filed);
        List<string> GetFiledTable(Table table, string filed);
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

        public void CreateTable(Table table)
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

        public void DeleteTable(Table table)
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

        public void AddColumn(Table table, TableColumn column)
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

        public void DeleteColumn(Table table, TableColumn column)
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

        public void UpdateLibraryPropertyInTable(Table table, string propertyName, string value)
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

        public void RenameColumn(Table table, TableColumn oldColumn, TableColumn newColumn)
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

        public List<string> GetFiledFromAllTables(List<Table> tables, string filed)
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
                    if (!reader.IsDBNull(0))
                        result.Add( reader.GetString(0));
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

        public List<string> GetFiledTable(Table table, string filed)
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
                    if (!reader.IsDBNull(0))
                        result.Add( reader.GetString(0));
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