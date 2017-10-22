using ReceiverDatabaseObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Logging;
namespace ReceiverDatabaseObjectsMySql
{
    public class ReceiverRelationalMySql : ReceiverRelational
    {
        public ReceiverRelationalMySql():base()
        {
            Name = "get details from mysql";
        }
        protected override async Task<KeyValuePair<string, string>[]> GetColumnsAsync(KeyValuePair<string, string> table, KeyValuePair<string, string> database)
        {
            string parameters = $"@TableName={table.Key}";
            string commandText = $"use {database.Value}" + @"          
                SELECT ordinal_position AS id, column_name AS name FROM information_schema.columns
                WHERE table_name = @TableName ORDER BY table_name,ordinal_position";
            return await FromCmd(commandText, parameters);
        }

        protected override async Task<KeyValuePair<string, string>[]> GetDatabasesAsync()
        {
            return await FromCmd("SELECT schema_name FROM information_schema.schemata;");
        }
        async Task<KeyValuePair<string, string>[]> FromCmd(string commandText, string parametersMappings = null)
        {

            var ret = new List<KeyValuePair<string, string>>();
            using (var cn = new MySqlConnection(ConnectionString))
            {
                await cn.OpenAsync();
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = cn;

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = commandText;

                    if (!string.IsNullOrEmpty(parametersMappings))
                    {
                        string[] parameters = parametersMappings.Split(';');

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            string paramName = parameters[i].Split('=')[0];
                            string value = parameters[i].Split('=')[1];
                            cmd.Parameters.AddWithValue(paramName, value);
                        }
                    }

                    using (var ir = cmd.ExecuteReader())
                    {
                        while (ir.Read())
                        {
                            var id = ir["ID"];
                            if (id == null || id == DBNull.Value)
                            {
                                string message = $"ID field in database is null";
                                //@class.Log(LogLevel.Information, 0, $"receiver relational sql server: {message}", null, null);                        
                                message += "";
                                continue;
                            }
                            ret.Add(new KeyValuePair<string, string>(ir["id"].ToString(), ir["name"].ToString()));
                        }
                    }
                }
            }
            return ret.ToArray();
        }
        protected override async Task<KeyValuePair<string, object>[]> GetServerDetailsAsync()
        {
            var ret = new List<KeyValuePair<string, object>>();
            var sb = new MySqlConnectionStringBuilder(ConnectionString);
            ret.Add(new KeyValuePair<string, object>("Name", sb.Server));
            ret.Add(new KeyValuePair<string, object>("ID", sb.Port));
            return await Task.FromResult(ret.ToArray());
        }

        protected override async Task<KeyValuePair<string, string>[]> GetTablesAsync(KeyValuePair<string, string> database)
        {
            string parameters = $"@tableName={database.Value}";
            string commandText = $"use {database.Value}" + @"
                SELECT TABLE_ID as id, name FROM INFORMATION_SCHEMA.INNODB_SYS_TABLES WHERE name LIKE @TableName;";
            return await FromCmd(commandText, parameters);
        }

        protected override async Task<KeyValuePair<string, string>[]> GetViewsAsync(KeyValuePair<string, string> database)
        {
            string parameters = $"@tableName={database.Value}";
            string commandText = $"use {database.Value}" + @"
                SELECT TABLE_SCHEMA, TABLE_NAME AS name FROM INFORMATION_SCHEMA.VIEWS WHERE table_name = @tableName";
            return await FromCmd(commandText);
        }
    }
}
