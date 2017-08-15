using System;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SenderDBStmtSqlServer
{
    /// <summary>
    /// TODO :
    /// Refactor ReceiverToDBStmtSqlServer and SenderToDBStmtSqlServer into a single class ExecuteDBStmtSqlServer
    /// Add XML transformer (rows -> XML scalar value) + map XML scalar value to a SP parameter
    /// Use SqlBulkCopy for high speed INSERTs
    /// Use table partitioning + partition SWITCH to import data (concurent INSERTs)
    /// </summary>
    public class SenderToDBStmtSqlServer : ISend
    {
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public string ParametersMappings { get; set; }
        private Dictionary<string, string> Parameters { get; set; }
        public bool HasParameters { get { return (this.Parameters != null && this.Parameters.Count > 0); } }

        public SenderToDBStmtSqlServer(string connectionString, CommandType commandType, string commandText, string parameterMappings = "")
        {
            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
            this.ParametersMappings = parameterMappings;
        }

        private void Initalization()
        {
            if (this.CommandType != CommandType.StoredProcedure)
                throw new NotImplementedException();

            //Example value for parameters: @param1=Col1;@param2=Col2;@param3=Col3
            //Where:
            //@param1,...   = {stored procedure|query} parameters
            //Col1,...      = columns serialized within lastRow
            if (!string.IsNullOrEmpty(this.ParametersMappings))
            {
                string[] parameters2 = this.ParametersMappings.Split(';');
                this.Parameters = new Dictionary<string, string>(parameters2.Length);
                for (int i = 0; i < parameters2.Length; i++)
                {
                    string paramName = parameters2[i].Split('=')[0];
                    string columnName = parameters2[i].Split('=')[1];
                    this.Parameters[paramName] = columnName;
                }
            }
        }

        public IRow[] valuesToBeSent { set; get; }

        public async Task Send()
        {
            //Initialization
            Initalization();

            //Send
            using (var conn = new SqlConnection(this.ConnectionString))
            {
                await conn.OpenAsync();

                foreach(var row in valuesToBeSent)
                {
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandType = this.CommandType;
                        cmd.CommandText = this.CommandText;
                        if (this.HasParameters)
                        {
                            foreach (var param in this.Parameters)
                            {
                                cmd.Parameters.AddWithValue(param.Key, row.Values[param.Value]);
                            }
                        }

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
}
