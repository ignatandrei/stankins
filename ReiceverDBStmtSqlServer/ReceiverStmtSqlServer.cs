using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Collections.Generic;
using StanskinsImplementation;
using System.Linq;

namespace ReiceverDBStmtSqlServer
{
    /// <summary>
    /// Note: Initial version will include support only for SPs calls (CommandType.StoredProcedure)
    /// TODO: Replace cmd.Parameters.AddWithValue with something else to avoid SQL Server plan cache pollution
    /// TODO: Refactor Dictionary<string, string> Parameters into a separate class
    /// Stored procedure (SP) with parameters (Dictionary<string, string> Parameters Parameters: Key is @sqlParamter, Value is columnName from SP resultset):
    ///     For the first call, we are call SP with default value of every paramter -> SP should be designed with this in mind; SP could be executed without parameters
    ///     When reading SP results, we are serializing values of the last row (we are serializing only values of columns from this.Parameters)
    ///     Next calls of the same SP will be made with values of the last row -> SP should be designed with this in mind
    ///     If resultset is empty then next SP call will be made with default values
    /// </summary>
    public class ReceiverStmtSqlServer : IReceive
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public string FileNameSerializeLastRow { get; set; }
        public string ParametersMappings { get; set; }
        private Dictionary<string, string> Parameters { get; set; } 
        public bool HasParameters { get { return (this.Parameters != null && this.Parameters.Count > 0); } }
        public bool SerializeLastRow { get { return (!string.IsNullOrEmpty(this.FileNameSerializeLastRow)); } }

        public ReceiverStmtSqlServer(string connectionString, CommandType commandType, string commandText, string fileNameSerializeLastRow, string parametersMappings = "")
        {
            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
            this.FileNameSerializeLastRow = fileNameSerializeLastRow;
            this.ParametersMappings = parametersMappings;
        }

        private void Initialization()
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
            if (HasParameters && string.IsNullOrEmpty(this.FileNameSerializeLastRow))
            {
                throw new Exception("If stored procedure has parameters then file name for serialization of last row is mandatory");
            }
        }

        public IRowReceive[] valuesRead { get; private set; }

        private Dictionary<string, object> lastRowValues;

        public async Task LoadData()
        {
            //Initialization
            Initialization();

            //Deserialize last received row
            if (this.SerializeLastRow)
            {
                using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastRow))
                {
                    lastRowValues = sdf.GetDictionary();
                }
            }

            List<RowRead> receivedRows = new List<RowRead>();

            using (var conn = new SqlConnection(this.ConnectionString))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = this.CommandType;
                    cmd.CommandText = this.CommandText;
                    if(this.HasParameters)
                    {
                        foreach(var param in this.Parameters)
                        {
                            if(lastRowValues.ContainsKey(param.Value))
                            {
                                cmd.Parameters.AddWithValue(param.Key, lastRowValues[param.Value]);
                            }
                        }
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

                        lastRowValues = row.Values; //For simplicity we are storing all column/values: now, we are not filtering only those columns from this.Parameters 
                    }
                }
            }

            valuesRead = receivedRows.ToArray();

            //Serialize last received row
            if (this.SerializeLastRow)
            {
                using (SerializeDataOnFile sdf = new SerializeDataOnFile(this.FileNameSerializeLastRow))
                {
                    //Only columns from this.Parameters are going to be serialized
                    Dictionary<string, object> selectedLastRowValues = this.HasParameters ? (lastRowValues.Where(filter => this.Parameters.Values.Contains(filter.Key)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)) : lastRowValues;
                    sdf.SetDictionary(selectedLastRowValues);
                }
            }
        }
    }
}
