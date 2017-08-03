using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Collections.Generic;
using StanskinsImplementation;

namespace ReiceverDBStmtSqlServer
{
    //Note: Initial version will include support only for SPs calls (CommandType.StoredProcedure)
    //TODO: Replace cmd.Parameters.AddWithValue with something else to avoid SQL Server plan cache pollution
    //TODO: Refactor Dictionary<string, string> Parameters into a separate class
    public class ReceiverStmtSqlServer : IReceive
    {
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public string FileNameSerializeLastRow { get; set; }
        public Dictionary<string, string> Parameters { get; private set; }
        public bool HasParameters { get { return (this.Parameters != null && this.Parameters.Count > 0); } }

        public ReceiverStmtSqlServer(string connectionString, CommandType commandType, string commandText, string fileNameSerializeLastRow, string parameters = "")
        {
            if (commandType != CommandType.StoredProcedure)
                throw new NotImplementedException();

            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
            
            this.FileNameSerializeLastRow = fileNameSerializeLastRow;
            //Example value for parameters: @param1=Col1;@param2=Col2;@param3=Col3
            //Where:
            //@param1,...   = {stored procedure|query} parameters
            //Col1,...      = columns serialized within lastRow
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] parameters2 = parameters.Split(';');
                this.Parameters = new Dictionary<string, string>(parameters2.Length);
                for (int i=0; i < parameters2.Length; i++)
                {
                    string paramName = parameters2[i].Split('=')[0];
                    string columnName = parameters2[i].Split('=')[1];
                    this.Parameters[paramName] = columnName;
                }
            }
            if (HasParameters && string.IsNullOrEmpty(this.FileNameSerializeLastRow))
            {
                throw new Exception("If stored procedure has parameters then file name for serialization of last row is mandatory");
            }
        }

        public IRowReceive[] valuesRead { get; private set; }

        private Dictionary<string, object> lastRowValues;

        public async Task LoadData()
        {
            //Deserialize last received row
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastRow))
            {
                lastRowValues = sdf.GetDictionary();
            }

            List<RowRead> receivedRows = new List<RowRead>();

            using (var conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = this.CommandText;
                    if(this.HasParameters)
                    {
                        foreach(var param in this.Parameters)
                        {
                            object paramValue = (lastRowValues.ContainsKey(param.Value) ? lastRowValues[param.Value] : DBNull.Value);
                            cmd.Parameters.AddWithValue(param.Key, paramValue);
                        }

                        cmd.CommandText += (' ' + string.Join(",", this.Parameters.Keys));
                    }
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

                        lastRowValues = row.Values;
                    }
                }
            }

            valuesRead = receivedRows.ToArray();

            //Serialize last received row
            using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastRow))
            {
                sdf.SetDictionary(lastRowValues);
            }
        }
    }
}
