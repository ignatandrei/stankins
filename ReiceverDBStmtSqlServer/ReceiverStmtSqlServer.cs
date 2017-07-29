using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Collections.Generic;
using StanskinsImplementation;

namespace ReiceverDBStmtSqlServer
{
    //Note: Initial version will include support only for SPs calls (CommandType.StoredProcedure
    //TODO: Add param; Map params (one or more) to LastValue/LastRow; Serialize LastVal/LastRow
    public enum StmtType : byte {ReadSqlScript, ExecStoredProcedure } 

    public class ReceiverStmtSqlServer : IReceive
    {
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }

        public ReceiverStmtSqlServer(string connectionString, CommandType commandType, string commandText)
        {
            if (commandType != CommandType.StoredProcedure)
                throw new NotImplementedException();

            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
        }

        public IRowReceive[] valuesRead { get; private set; }

        public async Task LoadData()
        {
            List<RowRead> receivedRows = new List<RowRead>();

            using (var conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = this.CommandText;
                    var rdr = await cmd.ExecuteReaderAsync();
                    while(rdr.Read())
                    {
                        RowRead row = new RowRead();
                        for(var i=0; i<rdr.FieldCount;i++)
                        {
                            string key = rdr.GetName(i);
                            object value = rdr[i];
                            row.Values.Add(key, value);
                        }
                        receivedRows.Add(row);
                    }
                }
            }

            valuesRead = receivedRows.ToArray();
        }
    }
}
