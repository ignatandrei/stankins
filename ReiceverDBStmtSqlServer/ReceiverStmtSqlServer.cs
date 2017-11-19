using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Collections.Generic;
using StanskinsImplementation;
using System.Linq;

namespace ReceiverDBStmtSqlServer
{
    /// <summary>
    /// Receiver for SQL Server/T-SQL batches (ex. stored procedure / ad-hoc query calls).
    /// </summary>
    public class ReceiverStmtSqlServer : IReceive
    {
        /// <summary>
        /// Gets or sets the name of the receiver.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the connection string used to connect to SQL Server instance.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Gets or sets a integer value indicating how the CommandText property is to be interpreted. Same as CommandType from System.Data.SqlClient namespace.
        /// </summary>
        public CommandType CommandType { get; set; }
        /// <summary>
        /// Gets or sets the Transact-SQL statement(s), table name or stored procedure to be executed on SQL Server instance.
        /// </summary>
        public string CommandText { get; set; }
        /// <summary>
        /// Gets or sets the path of *.json file used to serialize last received row. Last received row is serialized as Dictionary&lt;string,object&gt;.
        /// This property is property is optional. When it's empty string last received row is not serialized.
        /// </summary>
        public string FileNameSerializeLastRow { get; set; }
        /// <summary>
        /// <para>Gets or sets the mappings between the parameters of stored procedure and columns serialized into FileNameSerializeLastRow.</para>
        /// <para>Syntax: <code>@Parameter1=Column1;@Parameter2=Column2</code></para>
        /// <para><example><code>"FileNameSerializeLastRow": "active_slow_query_select_last_row.json",<lineBreak/>"ParametersMappings": "@original_id=original_id"</code></example></para>
        /// </summary>
        public string ParametersMappings { get; set; }
        /// <summary>
        /// Gets or sets the parameters of stored procedure. Created from ParametersMappings.
        /// </summary>
        private Dictionary<string, string> Parameters { get; set; }
        /// <summary>
        /// Returns true if Parameters has one or more items. Otherwise, it returns false.
        /// </summary>
        public bool HasParameters { get { return (this.Parameters != null && this.Parameters.Count > 0); } }
        /// <summary>
        /// Returns true if FileNameSerializeLastRow it isn't empty string.
        /// </summary>
        public bool SerializeLastRow { get { return (!string.IsNullOrEmpty(this.FileNameSerializeLastRow)); } }

        /// <summary>
        /// Initializes a new instance of the ReceiverStmtSqlServer class.
        /// </summary>
        /// <param name="connectionString">See ConnectionString property.</param>
        /// <param name="commandType">See CommandType property.</param>
        /// <param name="commandText">See CommandText property.</param>
        /// <param name="fileNameSerializeLastRow">See FileNameSerializeLastRow property.</param>
        /// <param name="parametersMappings">See ParametersMappings property.</param>
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

        /// <summary>
        /// Get or set the IRowReceive object used to store received values. If SerializeLastRow is true, the values from last received row are serialized into FileNameSerializeLastRow.
        /// </summary>
        public IRowReceive[] valuesRead { get; private set; }

        private Dictionary<string, object> lastRowValues;

        /// <summary>
        /// Execute the receiver filling valuesRead with received data.
        /// </summary>
        /// <returns></returns>
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
        public void ClearValues()
        {
            valuesRead = null;
        }
    }
}
