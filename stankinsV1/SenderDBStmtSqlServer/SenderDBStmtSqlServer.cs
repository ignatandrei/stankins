using System;
using System.Threading.Tasks;
using StankinsInterfaces;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SenderDBStmtSqlServer
{
    /// <summary>
    /// Sender to SQL Server.
    /// </summary>
    public class SenderToDBStmtSqlServer : ISend
    {
        /// <summary>
        /// Gets or sets the name of the sender.
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
        /// Gets or sets the mappings between the parameters of stored procedure and the source columns.
        /// Syntax: 
        ///     @Parameter1=SourceColumn1;@Parameter2=SourceColumn2
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
        /// Initializes a new instance of the SenderToDBStmtSqlServer class.
        /// </summary>
        /// <param name="connectionString">See <see cref="ConnectionString"/> property.</param>
        /// <param name="commandType">See <see cref="CommandType"/> property.</param>
        /// <param name="commandText">See <see cref="CommandText"/> property.</param>
        /// <param name="parameterMappings">See <see cref="ParametersMappings"/> property.</param>
        public SenderToDBStmtSqlServer(string connectionString, CommandType commandType, string commandText, string parameterMappings = "")
        {
            this.ConnectionString = connectionString;
            this.CommandType = commandType;
            this.CommandText = commandText;
            this.ParametersMappings = parameterMappings;
        }

        private void Initalization()
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
        }

        /// <summary>
        /// Get or set the IRow object used to store received values.
        /// </summary>
        public IRow[] valuesToBeSent { set; get; }

        /// <summary>
        /// Execute the sender reading data from valuesToBeSent.
        /// </summary>
        /// <returns></returns>
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
