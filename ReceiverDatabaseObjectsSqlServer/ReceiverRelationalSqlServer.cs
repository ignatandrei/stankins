using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace ReceiverDatabaseObjects
{
    public class ReceiverRelationalSqlServer : ReceiverRelational
    {
        public ReceiverRelationalSqlServer():base()
        {
            Name = "get details from sql server";
        }
        protected override async Task<KeyValuePair<string, object>[]> GetServerDetails()
        {
            var ret = new List<KeyValuePair<string, object>>();
            var sb = new SqlConnectionStringBuilder(ConnectionString);
            ret.Add(new KeyValuePair<string, object>( "Name", sb.DataSource));
            ret.Add(new KeyValuePair<string, object>("ID", sb.DataSource));
            return await Task.FromResult(ret.ToArray());
        }
        protected override async Task<KeyValuePair<string, string>[]> GetDatabases()
        {
            return await FromCmd("SELECT dbid as id,name FROM master.dbo.sysdatabases order by name");
        }
        async Task<KeyValuePair<string,string>[]> FromCmd(string commandText, string parametersMappings = null)
        {
            
            var ret = new List<KeyValuePair<string, string>>();
            using (var cn = new SqlConnection())
            {
                cn.ConnectionString = ConnectionString;
                await cn.OpenAsync();
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = cn;

                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = commandText;

                    //Example value for parameters: @param1=Col1;@param2=Col2;@param3=Col3
                    //Where:
                    //@param1,...   = {stored procedure|query} parameters
                    //Col1,...      = columns serialized within lastRow
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
                            if(id== null || id == DBNull.Value)
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
        protected override async Task<KeyValuePair<string, string>[]> GetTables(KeyValuePair<string, string> database)
        {
            string parameters = $"@tableName={database.Value}";
            string commandText = $"use {database.Value}"+@"
                SELECT TABLE_SCHEMA + '.'+TABLE_NAME as name , Object_id(TABLE_SCHEMA+'.'+TABLE_NAME) as id 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' and TABLE_CATALOG = @tableName
                order by TABLE_SCHEMA + '.'+TABLE_NAME";
            return await FromCmd(commandText,parameters);
        }
        protected override async Task<KeyValuePair<string, string>[]> GetViews(KeyValuePair<string, string> database)
        {
            //string parameters = $"@viewName={database.Value}";
            string commandText = $"use {database.Value}" + @"
                SELECT object_id as id, name 
                FROM sys.views  order by name";
            return await FromCmd(commandText);
        }
        protected override async Task<KeyValuePair<string, string>[]> GetColumns(KeyValuePair<string, string> table, KeyValuePair<string, string> database)
        {
            string parameters = $"@tableId={table.Key}";
            string commandText = $"use {database.Value}" + @"          
                SELECT column_id as id ,name
                FROM sys.columns
                where object_id= @tableId
                order by name";
            return await FromCmd(commandText, parameters);
        }
    }
}
