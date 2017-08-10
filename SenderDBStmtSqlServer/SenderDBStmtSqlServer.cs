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
        public string ConnectionString { get; private set; }
        public CommandType CommandType { get; private set; }
        public string CommandText { get; private set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public bool HasParameters { get { return (this.Parameters != null && this.Parameters.Count > 0); } }

        public SenderToDBStmtSqlServer(string connectionString, CommandType commandType, string commandText, string parameters = "")
        {
            if (commandType != CommandType.StoredProcedure)
                throw new NotImplementedException();

            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;

            //Example value for parameters: @param1=Col1;@param2=Col2;@param3=Col3
            //Where:
            //@param1,...   = {stored procedure|query} parameters
            //Col1,...      = columns serialized within lastRow
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] parameters2 = parameters.Split(';');
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
